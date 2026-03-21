using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Models;
using University_Admissions_Scoring_Engine.ViewModels;

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
                .OrderBy(g => g.IdAlgorytmGrupa)
                .ToListAsync();

            decimal wynik = 0m;

            foreach (var group in rootGroups)
            {
                wynik += await EvaluateGroup(group, dict);
            }

            return wynik;
        }

        public async Task<AlgorithmTestResult> EvaluateTestAsync(AlgorytmTestRequestViewModel request)
        {
            var algorytmMatura = await _context.AlgorytmyMatur
                .FirstOrDefaultAsync(a => a.AlgorytmId == request.AlgorytmId && a.MaturaId == request.MaturaId);

            if (algorytmMatura == null)
            {
                return new AlgorithmTestResult
                {
                    Points = 0m,
                    Lines = new List<string> { "Brak konfiguracji algorytmu dla wybranej matury." }
                };
            }

            var allGroups = await _context.AlgorytmGrupy
                .Include(g => g.AlgorytmOperacja)
                .Where(g => g.AlgorytmMaturaId == algorytmMatura.IdAlgorytmMatura)
                .OrderBy(g => g.IdAlgorytmGrupa)
                .ToListAsync();

            var allElements = await _context.AlgorytmLicze
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.Przedmiot)
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.PrzedmiotRodzaj)
                .Include(e => e.PrzedmiotRodzajPoziom)
                    .ThenInclude(p => p!.PrzedmiotPoziom)
                .Where(e => allGroups.Select(g => g.IdAlgorytmGrupa).Contains(e.AlgorytmGrupaId))
                .ToListAsync();

            var groupNumbers = allGroups
                .OrderBy(g => g.IdAlgorytmGrupa)
                .Select((g, i) => new { g.IdAlgorytmGrupa, Number = i })
                .ToDictionary(x => x.IdAlgorytmGrupa, x => x.Number);

            var inputs = request.Inputs
                .GroupBy(x => x.PrzedmiotRodzajPoziomId)
                .ToDictionary(g => g.Key, g => g.Last().Punkty);

            var rootGroups = allGroups
                .Where(g => g.RodzicId == null)
                .OrderBy(g => g.IdAlgorytmGrupa)
                .ToList();

            var finalLines = new List<string>();
            decimal total = 0m;

            foreach (var root in rootGroups)
            {
                var result = EvaluateGroupDetailed(root, allGroups, allElements, inputs, groupNumbers, 0);
                total += result.Points;
                finalLines.AddRange(result.Lines);
            }

            if (!finalLines.Any())
            {
                finalLines.Add("Brak danych wejściowych pasujących do algorytmu.");
            }

            return new AlgorithmTestResult
            {
                Points = total,
                Lines = finalLines
            };
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

        private GroupEvaluationResult EvaluateGroupDetailed(
            AlgorytmGrupa group,
            List<AlgorytmGrupa> allGroups,
            List<AlgorytmLicz> allElements,
            Dictionary<int, decimal> inputs,
            Dictionary<int, int> groupNumbers,
            int depth)
        {
            var indent = new string(' ', depth * 2);
            var operation = group.AlgorytmOperacja?.Nazwa ?? "BRAK";
            var groupNo = groupNumbers[group.IdAlgorytmGrupa];

            var ownElements = allElements
                .Where(e => e.AlgorytmGrupaId == group.IdAlgorytmGrupa)
                .ToList();

            var children = allGroups
                .Where(g => g.RodzicId == group.IdAlgorytmGrupa)
                .OrderBy(g => g.IdAlgorytmGrupa)
                .ToList();

            var candidates = new List<GroupEvaluationCandidate>();

            foreach (var el in ownElements)
            {
                if (!inputs.TryGetValue(el.PrzedmiotRodzajPoziomId, out var userPoints))
                    continue;

                var subjectLabel = $"{el.PrzedmiotRodzajPoziom?.Przedmiot?.Nazwa} / {el.PrzedmiotRodzajPoziom?.PrzedmiotRodzaj?.Nazwa} / {el.PrzedmiotRodzajPoziom?.PrzedmiotPoziom?.Nazwa}";
                var calculated = userPoints * el.Liczba;

                candidates.Add(new GroupEvaluationCandidate
                {
                    Points = calculated,
                    Lines = new List<string>
                    {
                        $"{indent}- {subjectLabel}: {userPoints:0.##} × {el.Liczba:0.##} = {calculated:0.##}"
                    }
                });
            }

            foreach (var child in children)
            {
                var childResult = EvaluateGroupDetailed(child, allGroups, allElements, inputs, groupNumbers, depth + 1);
                candidates.Add(new GroupEvaluationCandidate
                {
                    Points = childResult.Points,
                    Lines = childResult.Lines
                });
            }

            if (!candidates.Any())
            {
                return new GroupEvaluationResult
                {
                    Points = 0m,
                    Lines = new List<string>
                    {
                        $"{indent}Grupa {groupNo} [{operation}] → 0 (brak pasujących danych)"
                    }
                };
            }

            if (operation == "SUMA")
            {
                var total = candidates.Sum(x => x.Points);
                var lines = new List<string>
                {
                    $"{indent}Grupa {groupNo} [SUMA]"
                };

                foreach (var c in candidates)
                    lines.AddRange(c.Lines);

                lines.Add($"{indent}=> Wynik grupy {groupNo}: {total:0.##}");

                return new GroupEvaluationResult
                {
                    Points = total,
                    Lines = lines
                };
            }

            if (operation == "LUB")
            {
                var best = candidates
                    .OrderByDescending(x => x.Points)
                    .First();

                var lines = new List<string>
                {
                    $"{indent}Grupa {groupNo} [LUB] — wybrano najlepszą ścieżkę"
                };

                lines.AddRange(best.Lines);
                lines.Add($"{indent}=> Wynik grupy {groupNo}: {best.Points:0.##}");

                return new GroupEvaluationResult
                {
                    Points = best.Points,
                    Lines = lines
                };
            }

            return new GroupEvaluationResult
            {
                Points = 0m,
                Lines = new List<string>
                {
                    $"{indent}Grupa {groupNo} [{operation}] → 0 (nieznana operacja)"
                }
            };
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

    public class AlgorithmTestResult
    {
        public decimal Points { get; set; }
        public List<string> Lines { get; set; } = new();
    }

    internal class GroupEvaluationResult
    {
        public decimal Points { get; set; }
        public List<string> Lines { get; set; } = new();
    }

    internal class GroupEvaluationCandidate
    {
        public decimal Points { get; set; }
        public List<string> Lines { get; set; } = new();
    }
}