using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Services;
using University_Admissions_Scoring_Engine.ViewModels;

namespace University_Admissions_Scoring_Engine.Controllers
{
    public class LiczenieController : Controller
    {
        private readonly AppDbContext _context;
        private readonly AdmissionScoringService _scoringService;

        public LiczenieController(AppDbContext context, AdmissionScoringService scoringService)
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
                .OrderBy(k => k.Nazwa)
                .ToListAsync();

            return View(kierunki);
        }

        public async Task<IActionResult> Kierunek(int id)
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
                .Where(x => x.KierunekId == id)
                .OrderBy(x => x.Ranking)
                .ThenByDescending(x => x.Punkty)
                .ToListAsync();

            var vm = new KierunekLiczenieViewModel
            {
                Kierunek = kierunek,
                Kandydaci = kandydaci
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Przelicz(int kierunekId)
        {
            await _scoringService.CalculateForKierunekAsync(kierunekId);
            return RedirectToAction(nameof(Kierunek), new { id = kierunekId });
        }
    }
}