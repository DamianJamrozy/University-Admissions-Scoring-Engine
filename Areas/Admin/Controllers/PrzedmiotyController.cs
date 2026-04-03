using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Models;
using University_Admissions_Scoring_Engine.ViewModels;

namespace University_Admissions_Scoring_Engine.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PrzedmiotyController : Controller
    {
        private readonly AppDbContext _context;

        public PrzedmiotyController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var przedmioty = await _context.Przedmioty
                .OrderBy(x => x.Nazwa)
                .ToListAsync();

            return View(przedmioty);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PrzedmiotFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrzedmiotFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var exists = await _context.Przedmioty.AnyAsync(x => x.Nazwa == model.Nazwa.Trim());
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Nazwa), "Przedmiot o takiej nazwie już istnieje.");
                return View(model);
            }

            _context.Przedmioty.Add(new Przedmiot
            {
                Nazwa = model.Nazwa.Trim()
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Przedmiot został dodany.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.Przedmioty.FirstOrDefaultAsync(x => x.IdPrzedmiot == id);
            if (entity == null) return NotFound();

            var vm = new PrzedmiotFormViewModel
            {
                IdPrzedmiot = entity.IdPrzedmiot,
                Nazwa = entity.Nazwa
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PrzedmiotFormViewModel model)
        {
            if (!model.IdPrzedmiot.HasValue)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var entity = await _context.Przedmioty.FirstOrDefaultAsync(x => x.IdPrzedmiot == model.IdPrzedmiot.Value);
            if (entity == null) return NotFound();

            var exists = await _context.Przedmioty.AnyAsync(x => x.Nazwa == model.Nazwa.Trim() && x.IdPrzedmiot != model.IdPrzedmiot.Value);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Nazwa), "Przedmiot o takiej nazwie już istnieje.");
                return View(model);
            }

            entity.Nazwa = model.Nazwa.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = "Przedmiot został zaktualizowany.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Warianty()
        {
            var list = await _context.PrzedmiotRodzajPoziomy
                .Include(x => x.Przedmiot)
                .Include(x => x.PrzedmiotRodzaj)
                .Include(x => x.PrzedmiotPoziom)
                .OrderBy(x => x.Przedmiot!.Nazwa)
                .ThenBy(x => x.PrzedmiotRodzaj!.Nazwa)
                .ThenBy(x => x.PrzedmiotPoziom!.Nazwa)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateWariant()
        {
            var vm = new PrzedmiotWariantFormViewModel();
            await LoadLookupsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWariant(PrzedmiotWariantFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(model);
                return View(model);
            }

            var exists = await _context.PrzedmiotRodzajPoziomy.AnyAsync(x =>
                x.PrzedmiotId == model.PrzedmiotId &&
                x.PrzedmiotRodzajId == model.PrzedmiotRodzajId &&
                x.PrzedmiotPoziomId == model.PrzedmiotPoziomId);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Taki wariant przedmiotu już istnieje.");
                await LoadLookupsAsync(model);
                return View(model);
            }

            _context.PrzedmiotRodzajPoziomy.Add(new PrzedmiotRodzajPoziom
            {
                PrzedmiotId = model.PrzedmiotId,
                PrzedmiotRodzajId = model.PrzedmiotRodzajId,
                PrzedmiotPoziomId = model.PrzedmiotPoziomId
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Wariant przedmiotu został dodany.";
            return RedirectToAction(nameof(Warianty));
        }

        [HttpGet]
        public async Task<IActionResult> EditWariant(int id)
        {
            var entity = await _context.PrzedmiotRodzajPoziomy.FirstOrDefaultAsync(x => x.IdPrzedmiotRodzajPoziom == id);
            if (entity == null) return NotFound();

            var vm = new PrzedmiotWariantFormViewModel
            {
                IdPrzedmiotRodzajPoziom = entity.IdPrzedmiotRodzajPoziom,
                PrzedmiotId = entity.PrzedmiotId,
                PrzedmiotRodzajId = entity.PrzedmiotRodzajId,
                PrzedmiotPoziomId = entity.PrzedmiotPoziomId
            };

            await LoadLookupsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWariant(PrzedmiotWariantFormViewModel model)
        {
            if (!model.IdPrzedmiotRodzajPoziom.HasValue)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(model);
                return View(model);
            }

            var entity = await _context.PrzedmiotRodzajPoziomy
                .FirstOrDefaultAsync(x => x.IdPrzedmiotRodzajPoziom == model.IdPrzedmiotRodzajPoziom.Value);

            if (entity == null) return NotFound();

            var exists = await _context.PrzedmiotRodzajPoziomy.AnyAsync(x =>
                x.PrzedmiotId == model.PrzedmiotId &&
                x.PrzedmiotRodzajId == model.PrzedmiotRodzajId &&
                x.PrzedmiotPoziomId == model.PrzedmiotPoziomId &&
                x.IdPrzedmiotRodzajPoziom != model.IdPrzedmiotRodzajPoziom.Value);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Taki wariant przedmiotu już istnieje.");
                await LoadLookupsAsync(model);
                return View(model);
            }

            entity.PrzedmiotId = model.PrzedmiotId;
            entity.PrzedmiotRodzajId = model.PrzedmiotRodzajId;
            entity.PrzedmiotPoziomId = model.PrzedmiotPoziomId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Wariant przedmiotu został zaktualizowany.";
            return RedirectToAction(nameof(Warianty));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Przedmioty
                .Include(x => x.PrzedmiotRodzajPoziomy)
                .FirstOrDefaultAsync(x => x.IdPrzedmiot == id);

            if (entity == null) return NotFound();

            if (entity.PrzedmiotRodzajPoziomy.Any())
            {
                TempData["Error"] = "Nie można usunąć przedmiotu, ponieważ ma zdefiniowane warianty.";
                return RedirectToAction(nameof(Index));
            }

            _context.Przedmioty.Remove(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Przedmiot został usunięty.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWariant(int id)
        {
            var entity = await _context.PrzedmiotRodzajPoziomy
                .Include(x => x.MaturaPrzedmioty)
                .Include(x => x.AlgorytmLicze)
                .Include(x => x.KandydatDyplomPrzedmioty)
                .FirstOrDefaultAsync(x => x.IdPrzedmiotRodzajPoziom == id);

            if (entity == null) return NotFound();

            if (entity.MaturaPrzedmioty.Any() || entity.AlgorytmLicze.Any() || entity.KandydatDyplomPrzedmioty.Any())
            {
                TempData["Error"] = "Nie można usunąć wariantu, ponieważ jest używany w systemie.";
                return RedirectToAction(nameof(Warianty));
            }

            _context.PrzedmiotRodzajPoziomy.Remove(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Wariant przedmiotu został usunięty.";
            return RedirectToAction(nameof(Warianty));
        }

        private async Task LoadLookupsAsync(PrzedmiotWariantFormViewModel model)
        {
            model.Przedmioty = await _context.Przedmioty
                .OrderBy(x => x.Nazwa)
                .Select(x => new SelectListItem
                {
                    Value = x.IdPrzedmiot.ToString(),
                    Text = x.Nazwa
                })
                .ToListAsync();

            model.Rodzaje = await _context.PrzedmiotRodzaje
                .OrderBy(x => x.Nazwa)
                .Select(x => new SelectListItem
                {
                    Value = x.IdPrzedmiotRodzaj.ToString(),
                    Text = x.Nazwa
                })
                .ToListAsync();

            model.Poziomy = await _context.PrzedmiotPoziomy
                .OrderBy(x => x.Nazwa)
                .Select(x => new SelectListItem
                {
                    Value = x.IdPrzedmiotPoziom.ToString(),
                    Text = x.Nazwa
                })
                .ToListAsync();
        }
    }
}