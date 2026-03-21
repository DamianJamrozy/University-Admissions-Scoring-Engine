using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using University_Admissions_Scoring_Engine.Models;
using University_Admissions_Scoring_Engine.Services;

namespace University_Admissions_Scoring_Engine.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AdmissionScoringService _scoringService;

        public HomeController(
            ILogger<HomeController> logger,
            AdmissionScoringService scoringService)
        {
            _logger = logger;
            _scoringService = scoringService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // TEST LICZENIA
        // =========================
        public async Task<IActionResult> Test()
        {
            await _scoringService.CalculateForKierunekAsync(1);

            return Content("Policzono ranking dla kierunku o ID = 1");
        }

        // =========================
        // ERROR
        // =========================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}