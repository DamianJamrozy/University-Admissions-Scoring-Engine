using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Services;
using University_Admissions_Scoring_Engine.ViewModels;

namespace University_Admissions_Scoring_Engine.Controllers
{
    public class RankingiController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AdmissionScoringService _scoringService;

        public RankingiController(AppDbContext context, AdmissionScoringService scoringService)
        {
            _context = context;
            _scoringService = scoringService;
        }

        public async Task<IActionResult> Index()
        {
            var kierunki = await _context.Kierunki
                .Include(k => k.Tryb)
                .Include(k => k.Rodzaj)
                .Include(k => k.Algorytm)
                .Include(k => k.KandydatKierunki)
                .OrderBy(k => k.Nazwa)
                .ToListAsync();

            return View(kierunki);
        }

        public async Task<IActionResult> Details(int id)
        {
            var kierunek = await _context.Kierunki
                .Include(k => k.Tryb)
                .Include(k => k.Rodzaj)
                .Include(k => k.Algorytm)
                .FirstOrDefaultAsync(k => k.IdKierunek == id);

            if (kierunek == null)
                return NotFound();

            var kandydaci = await _context.KandydatKierunki
                .Include(x => x.Kandydat)
                .Include(x => x.Status)
                .Include(x => x.Kierunek)
                .Where(x => x.KierunekId == id)
                .OrderBy(x => x.Ranking ?? int.MaxValue)
                .ThenByDescending(x => x.Punkty ?? 0)
                .ToListAsync();

            var vm = new RankingDetailsViewModel
            {
                Kierunek = kierunek,
                KandydaciNaKierunek = kandydaci,
                LiczbaKandydatow = kandydaci.Count,
                CzyMaAlgorytm = kierunek.AlgorytmId.HasValue,
                NazwaAlgorytmu = kierunek.Algorytm?.Nazwa
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculatePoints(int kierunekId)
        {
            var kierunek = await _context.Kierunki
                .FirstOrDefaultAsync(k => k.IdKierunek == kierunekId);

            if (kierunek == null)
                return NotFound();

            if (!kierunek.AlgorytmId.HasValue)
            {
                TempData["Error"] = "Ten kierunek nie ma przypisanego algorytmu.";
                return RedirectToAction(nameof(Details), new { id = kierunekId });
            }

            await _scoringService.CalculatePointsOnlyAsync(kierunekId);

            TempData["Success"] = "Punkty kandydatów zostały przeliczone.";
            return RedirectToAction(nameof(Details), new { id = kierunekId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRanking(int kierunekId)
        {
            var kierunek = await _context.Kierunki
                .FirstOrDefaultAsync(k => k.IdKierunek == kierunekId);

            if (kierunek == null)
                return NotFound();

            await _scoringService.GenerateRankingOnlyAsync(kierunekId);

            TempData["Success"] = "Ranking i statusy zostały wygenerowane.";
            return RedirectToAction(nameof(Details), new { id = kierunekId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RebuildAll(int kierunekId)
        {
            var kierunek = await _context.Kierunki
                .FirstOrDefaultAsync(k => k.IdKierunek == kierunekId);

            if (kierunek == null)
                return NotFound();

            if (!kierunek.AlgorytmId.HasValue)
            {
                TempData["Error"] = "Ten kierunek nie ma przypisanego algorytmu.";
                return RedirectToAction(nameof(Details), new { id = kierunekId });
            }

            await _scoringService.CalculateForKierunekAsync(kierunekId);

            TempData["Success"] = "Przeliczono punkty oraz wygenerowano ranking.";
            return RedirectToAction(nameof(Details), new { id = kierunekId });
        }
    }
}