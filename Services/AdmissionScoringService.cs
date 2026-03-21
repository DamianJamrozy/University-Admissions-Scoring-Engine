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

            var wyniki = await _context.KandydatDyplomPrzedmioty
                .Where(w => w.KandydatDyplomId == dyplom.IdKandydatDyplom)
                .ToListAsync();

            var inputs = wyniki.ToDictionary(
                x => x.PrzedmiotRodzajPoziomId,
                x => x.Punkty
            );

            var groupNumbers = allGroups
                .OrderBy(g => g.IdAlgorytmGrupa)
                .Select((g, i) => new { g.IdAlgorytmGrupa, Number = i })
                .ToDictionary(x => x.IdAlgorytmGrupa, x => x.Number);

            var rootGroups = allGroups
                .Where(g => g.RodzicId == null)
                .OrderBy(g => g.IdAlgorytmGrupa)
                .ToList();

            decimal total = 0m;

            foreach (var root in rootGroups)
            {
                var result = EvaluateGroupDetailed(root, allGroups, allElements, inputs, groupNumbers, 0);
                total += result.Points;
            }

            return total;
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

            var groupLines = new List<string>
            {
                $"{indent}Grupa {groupNo} [{operation}]"
            };

            if (group.MinimalnePunkty.HasValue)
            {
                groupLines.Add($"{indent}  Minimum grupy: {group.MinimalnePunkty.Value:0.##}");
            }

            bool requirementFailed = false;
            var candidates = new List<GroupEvaluationCandidate>();

            // KLUCZOWA ZMIANA:
            // elementy grupujemy po przedmiocie i z jednego przedmiotu wybieramy najlepszą kombinację
            var subjectBlocks = ownElements
                .GroupBy(e => e.PrzedmiotRodzajPoziom!.PrzedmiotId)
                .ToList();

            foreach (var subjectBlock in subjectBlocks)
            {
                var subjectName = subjectBlock.First().PrzedmiotRodzajPoziom?.Przedmiot?.Nazwa ?? "-";
                var blockLines = new List<string>
                {
                    $"{indent}  Przedmiot: {subjectName}"
                };

                bool subjectRequired = subjectBlock.Any(x => x.CzyWymagany);
                var validVariants = new List<GroupEvaluationCandidate>();

                foreach (var el in subjectBlock)
                {
                    var label = $"{el.PrzedmiotRodzajPoziom?.PrzedmiotRodzaj?.Nazwa} / {el.PrzedmiotRodzajPoziom?.PrzedmiotPoziom?.Nazwa}";
                    var hasInput = inputs.TryGetValue(el.PrzedmiotRodzajPoziomId, out var rawPoints);

                    if (!hasInput)
                    {
                        blockLines.Add($"{indent}    - {label}: brak wyniku");
                        continue;
                    }

                    if (el.MinimalnePunkty.HasValue && rawPoints < el.MinimalnePunkty.Value)
                    {
                        blockLines.Add($"{indent}    - {label}: {rawPoints:0.##} < minimum {el.MinimalnePunkty.Value:0.##}");
                        continue;
                    }

                    var calculated = rawPoints * el.Liczba;

                    validVariants.Add(new GroupEvaluationCandidate
                    {
                        Points = calculated,
                        Lines = new List<string>
                        {
                            $"{indent}    - {label}: {rawPoints:0.##} × {el.Liczba:0.##} = {calculated:0.##}" +
                            $"{(el.CzyWymagany ? " [wymagany]" : "")}" +
                            $"{(el.MinimalnePunkty.HasValue ? $" [min {el.MinimalnePunkty.Value:0.##}]" : "")}"
                        }
                    });
                }

                if (!validVariants.Any())
                {
                    if (subjectRequired)
                    {
                        requirementFailed = true;
                        blockLines.Add($"{indent}    => brak poprawnej kombinacji dla wymaganego przedmiotu");
                    }
                    else
                    {
                        blockLines.Add($"{indent}    => brak poprawnej kombinacji, pominięto");
                    }

                    groupLines.AddRange(blockLines);
                    continue;
                }

                var bestVariant = validVariants
                    .OrderByDescending(x => x.Points)
                    .First();

                blockLines.Add($"{indent}    => wybrano najlepszą kombinację");
                blockLines.AddRange(bestVariant.Lines);

                candidates.Add(new GroupEvaluationCandidate
                {
                    Points = bestVariant.Points,
                    Lines = blockLines
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

            if (requirementFailed)
            {
                groupLines.Add($"{indent}=> Wynik grupy {groupNo}: 0 (niespełnione wymagania)");
                return new GroupEvaluationResult
                {
                    Points = 0m,
                    Lines = groupLines
                };
            }

            if (!candidates.Any())
            {
                groupLines.Add($"{indent}=> Wynik grupy {groupNo}: 0 (brak pasujących danych)");
                return new GroupEvaluationResult
                {
                    Points = 0m,
                    Lines = groupLines
                };
            }

            if (operation == "SUMA")
            {
                var total = candidates.Sum(x => x.Points);

                foreach (var c in candidates)
                    groupLines.AddRange(c.Lines);

                if (group.MinimalnePunkty.HasValue && total < group.MinimalnePunkty.Value)
                {
                    groupLines.Add($"{indent}=> Wynik grupy {groupNo}: 0 ({total:0.##} < minimum grupy {group.MinimalnePunkty.Value:0.##})");
                    return new GroupEvaluationResult
                    {
                        Points = 0m,
                        Lines = groupLines
                    };
                }

                groupLines.Add($"{indent}=> Wynik grupy {groupNo}: {total:0.##}");

                return new GroupEvaluationResult
                {
                    Points = total,
                    Lines = groupLines
                };
            }

            if (operation == "LUB")
            {
                var best = candidates
                    .OrderByDescending(x => x.Points)
                    .First();

                groupLines.Add($"{indent}  Wybrana najlepsza ścieżka:");
                groupLines.AddRange(best.Lines);

                if (group.MinimalnePunkty.HasValue && best.Points < group.MinimalnePunkty.Value)
                {
                    groupLines.Add($"{indent}=> Wynik grupy {groupNo}: 0 ({best.Points:0.##} < minimum grupy {group.MinimalnePunkty.Value:0.##})");
                    return new GroupEvaluationResult
                    {
                        Points = 0m,
                        Lines = groupLines
                    };
                }

                groupLines.Add($"{indent}=> Wynik grupy {groupNo}: {best.Points:0.##}");

                return new GroupEvaluationResult
                {
                    Points = best.Points,
                    Lines = groupLines
                };
            }

            groupLines.Add($"{indent}=> Wynik grupy {groupNo}: 0 (nieznana operacja)");
            return new GroupEvaluationResult
            {
                Points = 0m,
                Lines = groupLines
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