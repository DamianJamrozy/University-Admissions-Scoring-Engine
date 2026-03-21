using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Models;
using University_Admissions_Scoring_Engine.Services;
using University_Admissions_Scoring_Engine.ViewModels;

namespace University_Admissions_Scoring_Engine.Controllers
{
    public class AlgorytmyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AdmissionScoringService _scoringService;

        public AlgorytmyController(AppDbContext context, AdmissionScoringService scoringService)
        {
            _context = context;
            _scoringService = scoringService;
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
            var vm = await BuildEditorViewModelAsync(id, maturaId);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditorContent(int id, int? maturaId)
        {
            var vm = await BuildEditorViewModelAsync(id, maturaId);
            if (vm == null)
                return NotFound();

            return PartialView("_AlgorytmWorkspace", vm);
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
        public async Task<IActionResult> AddGroupInlineAjax(int algorytmMaturaId, int? rodzicId)
        {
            var operacja = await _context.AlgorytmOperacje
                .OrderBy(x => x.IdAlgorytmOperacja)
                .FirstAsync();

            var grupa = new AlgorytmGrupa
            {
                AlgorytmMaturaId = algorytmMaturaId,
                RodzicId = rodzicId,
                AlgorytmOperacjaId = operacja.IdAlgorytmOperacja,
                MinimalnePunkty = null
            };

            _context.AlgorytmGrupy.Add(grupa);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddElementInlineAjax(int groupId)
        {
            var grupa = await _context.AlgorytmGrupy.FindAsync(groupId);
            if (grupa == null)
                return Json(new { success = false });

            var first = await _context.PrzedmiotRodzajPoziomy
                .OrderBy(x => x.PrzedmiotId)
                .ThenBy(x => x.PrzedmiotRodzajId)
                .ThenBy(x => x.PrzedmiotPoziomId)
                .FirstAsync();

            var element = new AlgorytmLicz
            {
                AlgorytmGrupaId = groupId,
                PrzedmiotRodzajPoziomId = first.IdPrzedmiotRodzajPoziom,
                Liczba = 1m,
                CzyWymagany = false,
                MinimalnePunkty = null
            };

            _context.AlgorytmLicze.Add(element);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCombinationInlineAjax(int groupId, int subjectId)
        {
            var grupa = await _context.AlgorytmGrupy.FindAsync(groupId);
            if (grupa == null)
                return Json(new { success = false });

            var first = await _context.PrzedmiotRodzajPoziomy
                .Where(x => x.PrzedmiotId == subjectId)
                .OrderBy(x => x.PrzedmiotRodzajId)
                .ThenBy(x => x.PrzedmiotPoziomId)
                .FirstOrDefaultAsync();

            if (first == null)
                return Json(new { success = false });

            var element = new AlgorytmLicz
            {
                AlgorytmGrupaId = groupId,
                PrzedmiotRodzajPoziomId = first.IdPrzedmiotRodzajPoziom,
                Liczba = 1m,
                CzyWymagany = false,
                MinimalnePunkty = null
            };

            _context.AlgorytmLicze.Add(element);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteElementAjax(int elementId)
        {
            var element = await _context.AlgorytmLicze.FirstOrDefaultAsync(e => e.IdAlgorytmLicz == elementId);
            if (element == null)
                return Json(new { success = false });

            _context.AlgorytmLicze.Remove(element);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubjectBlockAjax(int groupId, int subjectId)
        {
            var elements = await _context.AlgorytmLicze
                .Include(x => x.PrzedmiotRodzajPoziom)
                .Where(x => x.AlgorytmGrupaId == groupId && x.PrzedmiotRodzajPoziom!.PrzedmiotId == subjectId)
                .ToListAsync();

            if (!elements.Any())
                return Json(new { success = false });

            _context.AlgorytmLicze.RemoveRange(elements);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroupAjax(int groupId)
        {
            var grupa = await _context.AlgorytmGrupy.FindAsync(groupId);
            if (grupa == null)
                return Json(new { success = false });

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

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAllAjax([FromBody] SaveAlgorithmRequest request)
        {
            if (request == null)
                return Json(new { success = false });

            if (request.GroupIds.Count != request.OperationIds.Count ||
                request.GroupIds.Count != request.GroupMinimums.Count)
                return Json(new { success = false });

            if (request.ElementIds.Count != request.PrzedmiotRodzajPoziomIds.Count ||
                request.ElementIds.Count != request.Liczby.Count ||
                request.ElementIds.Count != request.ElementRequiredFlags.Count ||
                request.ElementIds.Count != request.ElementMinimums.Count)
                return Json(new { success = false });

            var groups = await _context.AlgorytmGrupy
                .Where(x => request.GroupIds.Contains(x.IdAlgorytmGrupa))
                .ToListAsync();

            for (int i = 0; i < request.GroupIds.Count; i++)
            {
                var g = groups.FirstOrDefault(x => x.IdAlgorytmGrupa == request.GroupIds[i]);
                if (g == null)
                    continue;

                g.AlgorytmOperacjaId = request.OperationIds[i];
                g.MinimalnePunkty = request.GroupMinimums[i];
            }

            var elements = await _context.AlgorytmLicze
                .Where(x => request.ElementIds.Contains(x.IdAlgorytmLicz))
                .ToListAsync();

            for (int i = 0; i < request.ElementIds.Count; i++)
            {
                var e = elements.FirstOrDefault(x => x.IdAlgorytmLicz == request.ElementIds[i]);
                if (e == null)
                    continue;

                e.PrzedmiotRodzajPoziomId = request.PrzedmiotRodzajPoziomIds[i];
                e.Liczba = request.Liczby[i];
                e.CzyWymagany = request.ElementRequiredFlags[i];
                e.MinimalnePunkty = request.ElementMinimums[i];
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> TestAlgorithm([FromBody] AlgorytmTestRequestViewModel request)
        {
            var result = await _scoringService.EvaluateTestAsync(request);

            return Json(new
            {
                success = true,
                points = result.Points,
                lines = result.Lines
            });
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

        private async Task<AlgorytmEditorViewModel?> BuildEditorViewModelAsync(int id, int? maturaId)
        {
            var algorytm = await _context.Algorytmy
                .FirstOrDefaultAsync(a => a.IdAlgorytm == id);

            if (algorytm == null)
                return null;

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

            return new AlgorytmEditorViewModel
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

    public class SaveAlgorithmRequest
    {
        public int AlgorytmId { get; set; }
        public int MaturaId { get; set; }

        public List<int> GroupIds { get; set; } = new();
        public List<int> OperationIds { get; set; } = new();
        public List<decimal?> GroupMinimums { get; set; } = new();

        public List<int> ElementIds { get; set; } = new();
        public List<int> PrzedmiotRodzajPoziomIds { get; set; } = new();
        public List<decimal> Liczby { get; set; } = new();
        public List<bool> ElementRequiredFlags { get; set; } = new();
        public List<decimal?> ElementMinimums { get; set; } = new();
    }
}