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

        public async Task CalculateForKierunekAsync(int kierunekId)
        {
            var kandydaci = await _context.KandydatKierunki
                .Where(x => x.KierunekId == kierunekId)
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
                .FirstAsync(k => k.IdKierunek == kierunekId);

            var dyplom = await _context.KandydatDyplomy
                .Where(d => d.KandydatId == kandydatId)
                .OrderByDescending(d => d.Rok)
                .FirstAsync();

            var algorytmMatura = await _context.AlgorytmyMatur
                .FirstAsync(a =>
                    a.AlgorytmId == kierunek.AlgorytmId &&
                    a.MaturaId == dyplom.MaturaId);

            var wyniki = await _context.KandydatDyplomPrzedmioty
                .Where(w => w.KandydatDyplomId == dyplom.IdKandydatDyplom)
                .ToListAsync();

            var dict = wyniki.ToDictionary(
                x => x.PrzedmiotRodzajPoziomId,
                x => x.Punkty
            );

            var rootGroups = await _context.AlgorytmGrupy
                .Include(g => g.AlgorytmOperacja)
                .Where(g => g.AlgorytmMaturaId == algorytmMatura.IdAlgorytmMatura && g.RodzicId == null)
                .ToListAsync();

            decimal wynik = 0m;

            foreach (var group in rootGroups)
            {
                wynik += await EvaluateGroup(group, dict);
            }

            return wynik;
        }

        private async Task<decimal> EvaluateGroup(AlgorytmGrupa group, Dictionary<int, decimal> wyniki)
        {
            var operation = group.AlgorytmOperacja?.Nazwa ?? string.Empty;

            var dzieci = await _context.AlgorytmGrupy
                .Include(g => g.AlgorytmOperacja)
                .Where(g => g.RodzicId == group.IdAlgorytmGrupa)
                .ToListAsync();

            var elementy = await _context.AlgorytmLicze
                .Where(l => l.AlgorytmGrupaId == group.IdAlgorytmGrupa)
                .ToListAsync();

            var values = new List<decimal>();

            foreach (var el in elementy)
            {
                if (wyniki.TryGetValue(el.PrzedmiotRodzajPoziomId, out var val))
                {
                    values.Add(val * el.Liczba);
                }
            }

            foreach (var child in dzieci)
            {
                values.Add(await EvaluateGroup(child, wyniki));
            }

            if (!values.Any())
                return 0m;

            if (operation == "SUMA")
                return values.Sum();

            if (operation == "LUB")
                return values.Max();

            return 0m;
        }

        private async Task CalculateRankingAsync(int kierunekId)
        {
            var lista = await _context.KandydatKierunki
                .Where(k => k.KierunekId == kierunekId)
                .OrderByDescending(k => k.Punkty)
                .ToListAsync();

            var kierunek = await _context.Kierunki
                .FirstAsync(k => k.IdKierunek == kierunekId);

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