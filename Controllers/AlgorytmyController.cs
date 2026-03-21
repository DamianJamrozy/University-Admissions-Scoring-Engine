using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Details(int id)
        {
            var algorytm = await _context.Algorytmy
                .FirstOrDefaultAsync(a => a.IdAlgorytm == id);

            if (algorytm == null)
                return NotFound();

            var groups = await _context.AlgorytmGrupy
                .Include(g => g.AlgorytmOperacja)
                .Where(g => g.AlgorytmId == id)
                .OrderBy(g => g.IdAlgorytmGrupa)
                .ToListAsync();

            var elements = await _context.AlgorytmLicze
                .Include(e => e.AlgorytmGrupa)
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.Przedmiot)
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.PrzedmiotRodzaj)
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.PrzedmiotPoziom)
                .Where(e => e.AlgorytmGrupa!.AlgorytmId == id)
                .OrderBy(e => e.AlgorytmGrupaId)
                .ToListAsync();

            var vm = new ViewModels.AlgorytmDetailsViewModel
            {
                Algorytm = algorytm,
                Groups = groups,
                Elements = elements
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AddGroup(int algorytmId, int? rodzicId)
        {
            ViewBag.Operacje = new SelectList(
                await _context.AlgorytmOperacje.OrderBy(x => x.Nazwa).ToListAsync(),
                "IdAlgorytmOperacja",
                "Nazwa");

            var model = new AlgorytmGroupCreateViewModel
            {
                AlgorytmId = algorytmId,
                RodzicId = rodzicId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(AlgorytmGroupCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Operacje = new SelectList(
                    await _context.AlgorytmOperacje.OrderBy(x => x.Nazwa).ToListAsync(),
                    "IdAlgorytmOperacja",
                    "Nazwa",
                    model.AlgorytmOperacjaId);

                return View(model);
            }

            var grupa = new AlgorytmGrupa
            {
                AlgorytmId = model.AlgorytmId,
                RodzicId = model.RodzicId,
                AlgorytmOperacjaId = model.AlgorytmOperacjaId
            };

            _context.AlgorytmGrupy.Add(grupa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = model.AlgorytmId });
        }

        [HttpGet]
        public async Task<IActionResult> AddElement(int algorytmGrupaId)
        {
            var grupa = await _context.AlgorytmGrupy.FirstOrDefaultAsync(g => g.IdAlgorytmGrupa == algorytmGrupaId);
            if (grupa == null)
                return NotFound();

            var przedmioty = await _context.PrzedmiotRodzajPoziomy
                .Include(x => x.Przedmiot)
                .Include(x => x.PrzedmiotRodzaj)
                .Include(x => x.PrzedmiotPoziom)
                .OrderBy(x => x.Przedmiot!.Nazwa)
                .ToListAsync();

            ViewBag.Przedmioty = new SelectList(
                przedmioty.Select(x => new
                {
                    x.IdPrzedmiotRodzajPoziom,
                    Nazwa = $"{x.Przedmiot!.Nazwa} / {x.PrzedmiotRodzaj!.Nazwa} / {x.PrzedmiotPoziom!.Nazwa}"
                }),
                "IdPrzedmiotRodzajPoziom",
                "Nazwa");

            return View(new AlgorytmElementCreateViewModel
            {
                AlgorytmGrupaId = algorytmGrupaId,
                Liczba = 1m
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddElement(AlgorytmElementCreateViewModel model)
        {
            var grupa = await _context.AlgorytmGrupy.FirstOrDefaultAsync(g => g.IdAlgorytmGrupa == model.AlgorytmGrupaId);
            if (grupa == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var przedmioty = await _context.PrzedmiotRodzajPoziomy
                    .Include(x => x.Przedmiot)
                    .Include(x => x.PrzedmiotRodzaj)
                    .Include(x => x.PrzedmiotPoziom)
                    .OrderBy(x => x.Przedmiot!.Nazwa)
                    .ToListAsync();

                ViewBag.Przedmioty = new SelectList(
                    przedmioty.Select(x => new
                    {
                        x.IdPrzedmiotRodzajPoziom,
                        Nazwa = $"{x.Przedmiot!.Nazwa} / {x.PrzedmiotRodzaj!.Nazwa} / {x.PrzedmiotPoziom!.Nazwa}"
                    }),
                    "IdPrzedmiotRodzajPoziom",
                    "Nazwa",
                    model.PrzedmiotRodzajPoziomId);

                return View(model);
            }

            var element = new AlgorytmLicz
            {
                AlgorytmGrupaId = model.AlgorytmGrupaId,
                PrzedmiotRodzajPoziomId = model.PrzedmiotRodzajPoziomId,
                Liczba = model.Liczba
            };

            _context.AlgorytmLicze.Add(element);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = grupa.AlgorytmId });
        }
    }
}