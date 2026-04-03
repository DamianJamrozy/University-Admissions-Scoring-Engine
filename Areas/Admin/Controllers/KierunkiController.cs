using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Models;
using University_Admissions_Scoring_Engine.ViewModels;

namespace University_Admissions_Scoring_Engine.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KierunkiController : Controller
    {
        private readonly AppDbContext _context;

        public KierunkiController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kierunki = await _context.Kierunki
                .Include(k => k.Tryb)
                .Include(k => k.Rodzaj)
                .Include(k => k.Algorytm)
                .OrderBy(k => k.Nazwa)
                .ToListAsync();

            return View(kierunki);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new KierunekFormViewModel();
            await LoadLookupsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KierunekFormViewModel model)
        {
            if (model.MinPrzyjetych > model.MaxPrzyjetych)
            {
                ModelState.AddModelError(nameof(model.MinPrzyjetych), "Minimalna liczba przyjętych nie może być większa niż maksymalna.");
            }

            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(model);
                return View(model);
            }

            var kierunek = new Kierunek
            {
                Nazwa = model.Nazwa.Trim(),
                TrybId = model.TrybId,
                RodzajId = model.RodzajId,
                MinPrzyjetych = model.MinPrzyjetych,
                MaxPrzyjetych = model.MaxPrzyjetych,
                MaxListaRezerwowa = model.MaxListaRezerwowa,
                AlgorytmId = model.AlgorytmId
            };

            _context.Kierunki.Add(kierunek);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kierunek został dodany.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var kierunek = await _context.Kierunki.FirstOrDefaultAsync(k => k.IdKierunek == id);
            if (kierunek == null)
                return NotFound();

            var vm = new KierunekFormViewModel
            {
                IdKierunek = kierunek.IdKierunek,
                Nazwa = kierunek.Nazwa,
                TrybId = kierunek.TrybId,
                RodzajId = kierunek.RodzajId,
                MinPrzyjetych = kierunek.MinPrzyjetych,
                MaxPrzyjetych = kierunek.MaxPrzyjetych,
                MaxListaRezerwowa = kierunek.MaxListaRezerwowa,
                AlgorytmId = kierunek.AlgorytmId
            };

            await LoadLookupsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KierunekFormViewModel model)
        {
            if (!model.IdKierunek.HasValue)
                return NotFound();

            if (model.MinPrzyjetych > model.MaxPrzyjetych)
            {
                ModelState.AddModelError(nameof(model.MinPrzyjetych), "Minimalna liczba przyjętych nie może być większa niż maksymalna.");
            }

            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(model);
                return View(model);
            }

            var kierunek = await _context.Kierunki.FirstOrDefaultAsync(k => k.IdKierunek == model.IdKierunek.Value);
            if (kierunek == null)
                return NotFound();

            kierunek.Nazwa = model.Nazwa.Trim();
            kierunek.TrybId = model.TrybId;
            kierunek.RodzajId = model.RodzajId;
            kierunek.MinPrzyjetych = model.MinPrzyjetych;
            kierunek.MaxPrzyjetych = model.MaxPrzyjetych;
            kierunek.MaxListaRezerwowa = model.MaxListaRezerwowa;
            kierunek.AlgorytmId = model.AlgorytmId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Kierunek został zaktualizowany.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kierunek = await _context.Kierunki
                .Include(k => k.KandydatKierunki)
                .FirstOrDefaultAsync(k => k.IdKierunek == id);

            if (kierunek == null)
                return NotFound();

            if (kierunek.KandydatKierunki.Any())
            {
                TempData["Error"] = "Nie można usunąć kierunku, ponieważ są do niego przypisani kandydaci.";
                return RedirectToAction(nameof(Index));
            }

            _context.Kierunki.Remove(kierunek);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kierunek został usunięty.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync(KierunekFormViewModel model)
        {
            model.Tryby = await _context.KierunekTryby
                .OrderBy(x => x.Nazwa)
                .Select(x => new SelectListItem
                {
                    Value = x.IdTryb.ToString(),
                    Text = x.Nazwa
                })
                .ToListAsync();

            model.Rodzaje = await _context.KierunekRodzaje
                .OrderBy(x => x.Nazwa)
                .Select(x => new SelectListItem
                {
                    Value = x.IdRodzaj.ToString(),
                    Text = x.Nazwa
                })
                .ToListAsync();

            model.Algorytmy = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "-- brak --"
                }
            };

            var algorytmy = await _context.Algorytmy
                .OrderBy(x => x.Nazwa)
                .Select(x => new SelectListItem
                {
                    Value = x.IdAlgorytm.ToString(),
                    Text = $"{x.Nazwa} ({x.IdAlgorytm})"
                })
                .ToListAsync();

            model.Algorytmy.AddRange(algorytmy);
        }
    }
}