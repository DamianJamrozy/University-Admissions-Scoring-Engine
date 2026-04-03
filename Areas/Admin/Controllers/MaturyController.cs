using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Models;
using University_Admissions_Scoring_Engine.ViewModels;

namespace University_Admissions_Scoring_Engine.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MaturyController : Controller
    {
        private readonly AppDbContext _context;

        public MaturyController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var matury = await _context.Matury
                .OrderBy(x => x.Nazwa)
                .ToListAsync();

            return View(matury);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MaturaFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaturaFormViewModel model)
        {
            if (model.SkalaOd >= model.SkalaDo)
                ModelState.AddModelError(nameof(model.SkalaOd), "Skala od musi być mniejsza niż skala do.");

            if (model.SkalaUnit <= 0)
                ModelState.AddModelError(nameof(model.SkalaUnit), "Skok skali musi być większy od 0.");

            if (!ModelState.IsValid)
                return View(model);

            var exists = await _context.Matury.AnyAsync(x => x.Nazwa == model.Nazwa.Trim());
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Nazwa), "Matura o takiej nazwie już istnieje.");
                return View(model);
            }

            var entity = new Matura
            {
                Nazwa = model.Nazwa.Trim(),
                SkalaOd = model.SkalaOd,
                SkalaDo = model.SkalaDo,
                SkalaUnit = model.SkalaUnit
            };

            _context.Matury.Add(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Matura została dodana.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.Matury.FirstOrDefaultAsync(x => x.IdMatura == id);
            if (entity == null) return NotFound();

            var vm = new MaturaFormViewModel
            {
                IdMatura = entity.IdMatura,
                Nazwa = entity.Nazwa,
                SkalaOd = entity.SkalaOd,
                SkalaDo = entity.SkalaDo,
                SkalaUnit = entity.SkalaUnit
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MaturaFormViewModel model)
        {
            if (!model.IdMatura.HasValue)
                return NotFound();

            if (model.SkalaOd >= model.SkalaDo)
                ModelState.AddModelError(nameof(model.SkalaOd), "Skala od musi być mniejsza niż skala do.");

            if (model.SkalaUnit <= 0)
                ModelState.AddModelError(nameof(model.SkalaUnit), "Skok skali musi być większy od 0.");

            if (!ModelState.IsValid)
                return View(model);

            var entity = await _context.Matury.FirstOrDefaultAsync(x => x.IdMatura == model.IdMatura.Value);
            if (entity == null) return NotFound();

            var exists = await _context.Matury.AnyAsync(x => x.Nazwa == model.Nazwa.Trim() && x.IdMatura != model.IdMatura.Value);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Nazwa), "Matura o takiej nazwie już istnieje.");
                return View(model);
            }

            entity.Nazwa = model.Nazwa.Trim();
            entity.SkalaOd = model.SkalaOd;
            entity.SkalaDo = model.SkalaDo;
            entity.SkalaUnit = model.SkalaUnit;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Matura została zaktualizowana.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AssignSubjects(int id)
        {
            var matura = await _context.Matury.FirstOrDefaultAsync(x => x.IdMatura == id);
            if (matura == null) return NotFound();

            var wszystkie = await _context.PrzedmiotRodzajPoziomy
                .Include(x => x.Przedmiot)
                .Include(x => x.PrzedmiotRodzaj)
                .Include(x => x.PrzedmiotPoziom)
                .OrderBy(x => x.Przedmiot!.Nazwa)
                .ThenBy(x => x.PrzedmiotRodzaj!.Nazwa)
                .ThenBy(x => x.PrzedmiotPoziom!.Nazwa)
                .ToListAsync();

            var wybrane = await _context.MaturaPrzedmioty
                .Where(x => x.MaturaId == id)
                .Select(x => x.PrzedmiotRodzajPoziomId)
                .ToListAsync();

            var vm = new MaturaPrzedmiotyViewModel
            {
                Matura = matura,
                WszystkieWarianty = wszystkie,
                WybraneWariantyIds = wybrane
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSubjects(int maturaId, List<int> wybraneWariantyIds)
        {
            var matura = await _context.Matury.FirstOrDefaultAsync(x => x.IdMatura == maturaId);
            if (matura == null) return NotFound();

            wybraneWariantyIds ??= new List<int>();

            var existing = await _context.MaturaPrzedmioty
                .Where(x => x.MaturaId == maturaId)
                .ToListAsync();

            _context.MaturaPrzedmioty.RemoveRange(existing);

            var newRows = wybraneWariantyIds
                .Distinct()
                .Select(x => new MaturaPrzedmiot
                {
                    MaturaId = maturaId,
                    PrzedmiotRodzajPoziomId = x
                });

            await _context.MaturaPrzedmioty.AddRangeAsync(newRows);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Lista przedmiotów matury została zapisana.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Matury
                .Include(x => x.KandydatDyplomy)
                .Include(x => x.AlgorytmyMatur)
                .FirstOrDefaultAsync(x => x.IdMatura == id);

            if (entity == null) return NotFound();

            if (entity.KandydatDyplomy.Any() || entity.AlgorytmyMatur.Any())
            {
                TempData["Error"] = "Nie można usunąć matury, ponieważ jest używana w systemie.";
                return RedirectToAction(nameof(Index));
            }

            var assignments = await _context.MaturaPrzedmioty.Where(x => x.MaturaId == id).ToListAsync();
            _context.MaturaPrzedmioty.RemoveRange(assignments);
            _context.Matury.Remove(entity);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Matura została usunięta.";
            return RedirectToAction(nameof(Index));
        }
    }
}