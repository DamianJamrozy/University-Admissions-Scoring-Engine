using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Models;
using University_Admissions_Scoring_Engine.ViewModels;

namespace University_Admissions_Scoring_Engine.Controllers
{
    public class AlgorytmyController : Controller
    {
        private readonly AppDbContext _context;

        public AlgorytmyController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var algorytmy = await _context.Algorytmy
                .OrderBy(a => a.Nazwa)
                .ToListAsync();

            return View(algorytmy);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AlgorytmCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlgorytmCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var algorytm = new Algorytm
            {
                Nazwa = model.Nazwa
            };

            _context.Algorytmy.Add(algorytm);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = algorytm.IdAlgorytm });
        }

        public async Task<IActionResult> Details(int id, int? maturaId)
        {
            var algorytm = await _context.Algorytmy
                .FirstOrDefaultAsync(a => a.IdAlgorytm == id);

            if (algorytm == null)
                return NotFound();

            var matury = await _context.Matury
                .OrderBy(x => x.IdMatura)
                .ToListAsync();

            var selectedMaturaId = maturaId ?? matury.First().IdMatura;

            var algorytmMatura = await _context.AlgorytmyMatur
                .FirstOrDefaultAsync(x => x.AlgorytmId == id && x.MaturaId == selectedMaturaId);

            if (algorytmMatura == null)
            {
                algorytmMatura = new AlgorytmMatura
                {
                    AlgorytmId = id,
                    MaturaId = selectedMaturaId
                };

                _context.AlgorytmyMatur.Add(algorytmMatura);
                await _context.SaveChangesAsync();
            }

            var groups = await _context.AlgorytmGrupy
                .Include(g => g.AlgorytmOperacja)
                .Where(g => g.AlgorytmMaturaId == algorytmMatura.IdAlgorytmMatura)
                .OrderBy(g => g.IdAlgorytmGrupa)
                .ToListAsync();

            var groupIds = groups.Select(g => g.IdAlgorytmGrupa).ToList();

            var elements = await _context.AlgorytmLicze
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.Przedmiot)
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.PrzedmiotRodzaj)
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.PrzedmiotPoziom)
                .Where(e => groupIds.Contains(e.AlgorytmGrupaId))
                .OrderBy(e => e.IdAlgorytmLicz)
                .ToListAsync();

            var operacje = await _context.AlgorytmOperacje
                .OrderBy(x => x.Nazwa)
                .ToListAsync();

            var przedmioty = await _context.PrzedmiotRodzajPoziomy
                .Include(x => x.Przedmiot)
                .Include(x => x.PrzedmiotRodzaj)
                .Include(x => x.PrzedmiotPoziom)
                .OrderBy(x => x.Przedmiot!.Nazwa)
                .ThenBy(x => x.PrzedmiotRodzaj!.Nazwa)
                .ThenBy(x => x.PrzedmiotPoziom!.Nazwa)
                .ToListAsync();

            var vm = new AlgorytmEditorViewModel
            {
                Algorytm = algorytm,
                SelectedMaturaId = selectedMaturaId,
                AlgorytmMaturaId = algorytmMatura.IdAlgorytmMatura,
                Matury = matury,
                Groups = groups,
                Elements = elements,
                Operacje = operacje,
                PrzedmiotOpcje = przedmioty
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AssignToKierunki(int id)
        {
            var algorytm = await _context.Algorytmy
                .FirstOrDefaultAsync(x => x.IdAlgorytm == id);

            if (algorytm == null)
                return NotFound();

            var kierunki = await _context.Kierunki
                .Include(x => x.Tryb)
                .Include(x => x.Rodzaj)
                .OrderBy(x => x.Nazwa)
                .ToListAsync();

            var vm = new AlgorytmKierunkiViewModel
            {
                Algorytm = algorytm,
                WszystkieKierunki = kierunki,
                WybraneKierunkiIds = kierunki
                    .Where(x => x.AlgorytmId == algorytm.IdAlgorytm)
                    .Select(x => x.IdKierunek)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToKierunki(AlgorytmKierunkiViewModel model)
        {
            var algorytm = await _context.Algorytmy
                .FirstOrDefaultAsync(x => x.IdAlgorytm == model.Algorytm.IdAlgorytm);

            if (algorytm == null)
                return NotFound();

            var wszystkieKierunki = await _context.Kierunki.ToListAsync();

            foreach (var kierunek in wszystkieKierunki)
            {
                if (model.WybraneKierunkiIds.Contains(kierunek.IdKierunek))
                {
                    kierunek.AlgorytmId = algorytm.IdAlgorytm;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Przypisania algorytmu zostały zapisane.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroupInline(int algorytmMaturaId, int? rodzicId)
        {
            var operacja = await _context.AlgorytmOperacje
                .OrderBy(x => x.IdAlgorytmOperacja)
                .FirstAsync();

            var grupa = new AlgorytmGrupa
            {
                AlgorytmMaturaId = algorytmMaturaId,
                RodzicId = rodzicId,
                AlgorytmOperacjaId = operacja.IdAlgorytmOperacja
            };

            _context.AlgorytmGrupy.Add(grupa);
            await _context.SaveChangesAsync();

            var alg = await _context.AlgorytmyMatur.FindAsync(algorytmMaturaId);

            return RedirectToAction(nameof(Details), new { id = alg!.AlgorytmId, maturaId = alg.MaturaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddElementInline(int groupId)
        {
            var grupa = await _context.AlgorytmGrupy.FindAsync(groupId);
            if (grupa == null)
                return NotFound();

            var first = await _context.PrzedmiotRodzajPoziomy
                .OrderBy(x => x.IdPrzedmiotRodzajPoziom)
                .FirstAsync();

            var element = new AlgorytmLicz
            {
                AlgorytmGrupaId = groupId,
                PrzedmiotRodzajPoziomId = first.IdPrzedmiotRodzajPoziom,
                Liczba = 1m
            };

            _context.AlgorytmLicze.Add(element);
            await _context.SaveChangesAsync();

            var alg = await _context.AlgorytmyMatur.FindAsync(grupa.AlgorytmMaturaId);

            return RedirectToAction(nameof(Details), new { id = alg!.AlgorytmId, maturaId = alg.MaturaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGroupOperation(int groupId, int operationId)
        {
            var grupa = await _context.AlgorytmGrupy.FindAsync(groupId);
            if (grupa == null)
                return NotFound();

            grupa.AlgorytmOperacjaId = operationId;
            await _context.SaveChangesAsync();

            var alg = await _context.AlgorytmyMatur.FindAsync(grupa.AlgorytmMaturaId);

            return RedirectToAction(nameof(Details), new { id = alg!.AlgorytmId, maturaId = alg.MaturaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAll(
            int algorytmId,
            int maturaId,
            List<int> elementIds,
            List<int> przedmiotRodzajPoziomIds,
            List<decimal> liczby)
        {
            if (elementIds.Count != przedmiotRodzajPoziomIds.Count || elementIds.Count != liczby.Count)
            {
                return RedirectToAction(nameof(Details), new { id = algorytmId, maturaId });
            }

            var ids = elementIds.Distinct().ToList();

            var elements = await _context.AlgorytmLicze
                .Where(x => ids.Contains(x.IdAlgorytmLicz))
                .ToListAsync();

            for (int i = 0; i < elementIds.Count; i++)
            {
                var el = elements.FirstOrDefault(x => x.IdAlgorytmLicz == elementIds[i]);
                if (el == null)
                    continue;

                el.PrzedmiotRodzajPoziomId = przedmiotRodzajPoziomIds[i];
                el.Liczba = liczby[i];
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = algorytmId, maturaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteElement(int elementId)
        {
            var element = await _context.AlgorytmLicze
                .Include(e => e.AlgorytmGrupa)
                .FirstOrDefaultAsync(e => e.IdAlgorytmLicz == elementId);

            if (element == null)
                return NotFound();

            var alg = await _context.AlgorytmyMatur.FindAsync(element.AlgorytmGrupa!.AlgorytmMaturaId);

            _context.AlgorytmLicze.Remove(element);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = alg!.AlgorytmId, maturaId = alg.MaturaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var grupa = await _context.AlgorytmGrupy.FindAsync(groupId);
            if (grupa == null)
                return NotFound();

            var ids = await GetGroupBranch(groupId);

            var elements = await _context.AlgorytmLicze
                .Where(e => ids.Contains(e.AlgorytmGrupaId))
                .ToListAsync();

            var groups = await _context.AlgorytmGrupy
                .Where(g => ids.Contains(g.IdAlgorytmGrupa))
                .ToListAsync();

            _context.AlgorytmLicze.RemoveRange(elements);
            _context.AlgorytmGrupy.RemoveRange(groups);
            await _context.SaveChangesAsync();

            var alg = await _context.AlgorytmyMatur.FindAsync(grupa.AlgorytmMaturaId);

            return RedirectToAction(nameof(Details), new { id = alg!.AlgorytmId, maturaId = alg.MaturaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var algorytm = await _context.Algorytmy
                .FirstOrDefaultAsync(x => x.IdAlgorytm == id);

            if (algorytm == null)
                return NotFound();

            var isUsed = await _context.Kierunki.AnyAsync(x => x.AlgorytmId == id);
            if (isUsed)
            {
                TempData["Error"] = "Nie można usunąć algorytmu, ponieważ jest przypisany do co najmniej jednego kierunku.";
                return RedirectToAction(nameof(Index));
            }

            var algMatury = await _context.AlgorytmyMatur
                .Where(x => x.AlgorytmId == id)
                .ToListAsync();

            var algMaturyIds = algMatury.Select(x => x.IdAlgorytmMatura).ToList();

            var groups = await _context.AlgorytmGrupy
                .Where(x => algMaturyIds.Contains(x.AlgorytmMaturaId))
                .ToListAsync();

            var groupIds = groups.Select(x => x.IdAlgorytmGrupa).ToList();

            var elements = await _context.AlgorytmLicze
                .Where(x => groupIds.Contains(x.AlgorytmGrupaId))
                .ToListAsync();

            _context.AlgorytmLicze.RemoveRange(elements);
            _context.AlgorytmGrupy.RemoveRange(groups);
            _context.AlgorytmyMatur.RemoveRange(algMatury);
            _context.Algorytmy.Remove(algorytm);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Algorytm został usunięty.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<int>> GetGroupBranch(int id)
        {
            var all = await _context.AlgorytmGrupy.ToListAsync();
            var result = new List<int>();

            void Walk(int gid)
            {
                result.Add(gid);

                var children = all
                    .Where(x => x.RodzicId == gid)
                    .Select(x => x.IdAlgorytmGrupa)
                    .ToList();

                foreach (var c in children)
                    Walk(c);
            }

            Walk(id);

            return result;
        }
    }
}