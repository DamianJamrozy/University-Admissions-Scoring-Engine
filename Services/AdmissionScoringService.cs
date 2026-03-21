using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.Services
{
    public class AdmissionScoringService
    {
        private readonly AppDbContext _context;

        public AdmissionScoringService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // PUBLIC API
        // =========================

        public async Task CalculateForKierunekAsync(int kierunekId)
        {
            var kierunek = await _context.Kierunki
                .Include(k => k.Algorytm)
                .FirstAsync(k => k.IdKierunek == kierunekId);

            var kandydaci = await _context.KandydatKierunki
                .Where(k => k.KierunekId == kierunekId)
                .ToListAsync();

            foreach (var kk in kandydaci)
            {
                var punkty = await CalculateForKandydatAsync(kk.KandydatId, kierunekId);
                kk.Punkty = punkty;
            }

            await _context.SaveChangesAsync();

            await CalculateRankingAsync(kierunekId);
        }

        public async Task<decimal> CalculateForKandydatAsync(int kandydatId, int kierunekId)
        {
            var kierunek = await _context.Kierunki
                .Include(k => k.Algorytm)
                .FirstAsync(k => k.IdKierunek == kierunekId);

            var algorytm = await _context.Algorytmy
                .Include(a => a.AlgorytmGrupy)
                .FirstAsync(a => a.IdAlgorytm == kierunek.AlgorytmId);

            var wyniki = await GetWynikiKandydata(kandydatId);

            var rootGroups = await _context.AlgorytmGrupy
                .Where(g => g.AlgorytmId == algorytm.IdAlgorytm && g.RodzicId == null)
                .Include(g => g.AlgorytmOperacja)
                .ToListAsync();

            decimal wynik = 0;

            foreach (var group in rootGroups)
            {
                wynik += await EvaluateGroup(group, wyniki);
            }

            return wynik;
        }

        // =========================
        // REKURENCJA ALGORYTMU
        // =========================

        private async Task<decimal> EvaluateGroup(AlgorytmGrupa group, Dictionary<int, decimal> wyniki)
        {
            var operation = group.AlgorytmOperacja!.Nazwa;

            var dzieci = await _context.AlgorytmGrupy
                .Where(g => g.RodzicId == group.IdAlgorytmGrupa)
                .Include(g => g.AlgorytmOperacja)
                .ToListAsync();

            var elementy = await _context.AlgorytmLicze
                .Where(l => l.AlgorytmGrupaId == group.IdAlgorytmGrupa)
                .ToListAsync();

            var values = new List<decimal>();

            // przedmioty
            foreach (var el in elementy)
            {
                if (wyniki.TryGetValue(el.PrzedmiotRodzajPoziomId, out var val))
                {
                    values.Add(val * el.Liczba);
                }
            }

            // podgrupy
            foreach (var child in dzieci)
            {
                var val = await EvaluateGroup(child, wyniki);
                values.Add(val);
            }

            if (!values.Any())
                return 0;

            if (operation == "SUMA")
                return values.Sum();

            if (operation == "LUB")
                return values.Max();

            return 0;
        }

        // =========================
        // WYNIKI KANDYDATA
        // =========================

        private async Task<Dictionary<int, decimal>> GetWynikiKandydata(int kandydatId)
        {
            var dyplom = await _context.KandydatDyplomy
                .Where(d => d.KandydatId == kandydatId)
                .OrderByDescending(d => d.Rok)
                .FirstAsync();

            var wyniki = await _context.KandydatDyplomPrzedmioty
                .Where(w => w.KandydatDyplomId == dyplom.IdKandydatDyplom)
                .ToListAsync();

            return wyniki.ToDictionary(
                x => x.PrzedmiotRodzajPoziomId,
                x => x.Punkty
            );
        }

        // =========================
        // RANKING + STATUSY
        // =========================

        private async Task CalculateRankingAsync(int kierunekId)
        {
            var lista = await _context.KandydatKierunki
                .Where(k => k.KierunekId == kierunekId)
                .OrderByDescending(k => k.Punkty)
                .ToListAsync();

            var kierunek = await _context.Kierunki.FirstAsync(k => k.IdKierunek == kierunekId);

            var statusy = await _context.Statusy.ToListAsync();

            var przyjety = statusy.First(s => s.Nazwa == "Przyjęty");
            var rezerwowa = statusy.First(s => s.Nazwa == "Lista rezerwowa");
            var odrzucony = statusy.First(s => s.Nazwa == "Niezakwalifikowany");

            for (int i = 0; i < lista.Count; i++)
            {
                var item = lista[i];

                item.Ranking = i + 1;

                if (i < kierunek.MaxPrzyjetych)
                    item.StatusId = przyjety.IdStatus;
                else if (i < kierunek.MaxPrzyjetych + kierunek.MaxListaRezerwowa)
                    item.StatusId = rezerwowa.IdStatus;
                else
                    item.StatusId = odrzucony.IdStatus;
            }

            await _context.SaveChangesAsync();
        }
    }
}