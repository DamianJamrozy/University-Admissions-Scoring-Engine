using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();
            await SeedAsync(context);
        }

        private static async Task SeedAsync(AppDbContext context)
        {
            // ======================
            // STATUSY
            // ======================
            if (!await context.Statusy.AnyAsync())
            {
                context.Statusy.AddRange(
                    new Status { Nazwa = "Przyjęty" },
                    new Status { Nazwa = "Lista rezerwowa" },
                    new Status { Nazwa = "Niezakwalifikowany" }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // RODZAJE PRZEDMIOTÓW
            // ======================
            if (!await context.PrzedmiotRodzaje.AnyAsync())
            {
                context.PrzedmiotRodzaje.AddRange(
                    new PrzedmiotRodzaj { Nazwa = "Ustny" },
                    new PrzedmiotRodzaj { Nazwa = "Pisemny" }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // POZIOMY PRZEDMIOTÓW
            // ======================
            if (!await context.PrzedmiotPoziomy.AnyAsync())
            {
                context.PrzedmiotPoziomy.AddRange(
                    new PrzedmiotPoziom { Nazwa = "Podstawowa" },
                    new PrzedmiotPoziom { Nazwa = "Rozszerzenie" }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // TRYBY KIERUNKÓW
            // ======================
            if (!await context.KierunekTryby.AnyAsync())
            {
                context.KierunekTryby.AddRange(
                    new KierunekTryb { Nazwa = "Stacjonarne" },
                    new KierunekTryb { Nazwa = "Niestacjonarne" }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // RODZAJE KIERUNKÓW
            // ======================
            if (!await context.KierunekRodzaje.AnyAsync())
            {
                context.KierunekRodzaje.AddRange(
                    new KierunekRodzaj { Nazwa = "I stopnia" },
                    new KierunekRodzaj { Nazwa = "II stopnia" },
                    new KierunekRodzaj { Nazwa = "Jednolite mgr" }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // OPERACJE ALGORYTMÓW
            // ======================
            if (!await context.AlgorytmOperacje.AnyAsync())
            {
                context.AlgorytmOperacje.AddRange(
                    new AlgorytmOperacja { Nazwa = "SUMA" },
                    new AlgorytmOperacja { Nazwa = "LUB" }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // TYPY MATUR
            // ======================
            if (!await context.Matury.AnyAsync())
            {
                context.Matury.AddRange(
                    new Matura
                    {
                        Nazwa = "Nowa matura",
                        SkalaOd = 0m,
                        SkalaDo = 100m,
                        SkalaUnit = 1m
                    },
                    new Matura
                    {
                        Nazwa = "Stara matura",
                        SkalaOd = 1m,
                        SkalaDo = 6m,
                        SkalaUnit = 0.5m
                    }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // PRZEDMIOTY
            // ======================
            if (!await context.Przedmioty.AnyAsync())
            {
                var przedmioty = new[]
                {
            "Język polski",
            "Matematyka",
            "Język angielski",
            "Język niemiecki",
            "Język francuski",
            "Język hiszpański",
            "Język rosyjski",
            "Język włoski",
            "Język łaciński i kultura antyczna",
            "Biologia",
            "Chemia",
            "Fizyka",
            "Astronomia",
            "Fizyka i astronomia",
            "Geografia",
            "Historia",
            "Historia muzyki",
            "Historia sztuki",
            "Informatyka",
            "Wiedza o społeczeństwie",
            "Filozofia"
        };

                foreach (var nazwa in przedmioty)
                {
                    context.Przedmioty.Add(new Przedmiot
                    {
                        Nazwa = nazwa
                    });
                }

                await context.SaveChangesAsync();
            }

            // ======================
            // KONFIGURACJE PRZEDMIOT + RODZAJ + POZIOM
            // ======================
            if (!await context.PrzedmiotRodzajPoziomy.AnyAsync())
            {
                var przedmioty = await context.Przedmioty.AsNoTracking().ToListAsync();
                var rodzaje = await context.PrzedmiotRodzaje.AsNoTracking().ToListAsync();
                var poziomy = await context.PrzedmiotPoziomy.AsNoTracking().ToListAsync();

                var ustnyId = rodzaje.Single(x => x.Nazwa == "Ustny").IdPrzedmiotRodzaj;
                var pisemnyId = rodzaje.Single(x => x.Nazwa == "Pisemny").IdPrzedmiotRodzaj;
                var podstawowaId = poziomy.Single(x => x.Nazwa == "Podstawowa").IdPrzedmiotPoziom;
                var rozszerzenieId = poziomy.Single(x => x.Nazwa == "Rozszerzenie").IdPrzedmiotPoziom;

                var jezykiNowozytne = new HashSet<string>
        {
            "Język angielski",
            "Język niemiecki",
            "Język francuski",
            "Język hiszpański",
            "Język rosyjski",
            "Język włoski"
        };

                foreach (var przedmiot in przedmioty)
                {
                    // Pisemny podstawowy
                    context.PrzedmiotRodzajPoziomy.Add(new PrzedmiotRodzajPoziom
                    {
                        PrzedmiotId = przedmiot.IdPrzedmiot,
                        PrzedmiotRodzajId = pisemnyId,
                        PrzedmiotPoziomId = podstawowaId
                    });

                    // Pisemny rozszerzony
                    context.PrzedmiotRodzajPoziomy.Add(new PrzedmiotRodzajPoziom
                    {
                        PrzedmiotId = przedmiot.IdPrzedmiot,
                        PrzedmiotRodzajId = pisemnyId,
                        PrzedmiotPoziomId = rozszerzenieId
                    });

                    // Ustny głównie dla języka polskiego i języków obcych
                    if (przedmiot.Nazwa == "Język polski" || jezykiNowozytne.Contains(przedmiot.Nazwa))
                    {
                        context.PrzedmiotRodzajPoziomy.Add(new PrzedmiotRodzajPoziom
                        {
                            PrzedmiotId = przedmiot.IdPrzedmiot,
                            PrzedmiotRodzajId = ustnyId,
                            PrzedmiotPoziomId = podstawowaId
                        });

                        context.PrzedmiotRodzajPoziomy.Add(new PrzedmiotRodzajPoziom
                        {
                            PrzedmiotId = przedmiot.IdPrzedmiot,
                            PrzedmiotRodzajId = ustnyId,
                            PrzedmiotPoziomId = rozszerzenieId
                        });
                    }
                }

                await context.SaveChangesAsync();
            }

            // ======================
            // POWIĄZANIA MATURA -> PRZEDMIOTY
            // ======================
            if (!await context.MaturaPrzedmioty.AnyAsync())
            {
                var matury = await context.Matury.AsNoTracking().ToListAsync();
                var konfiguracje = await context.PrzedmiotRodzajPoziomy
                    .Include(x => x.Przedmiot)
                    .Include(x => x.PrzedmiotRodzaj)
                    .Include(x => x.PrzedmiotPoziom)
                    .AsNoTracking()
                    .ToListAsync();

                var nowaMatura = matury.Single(x => x.Nazwa == "Nowa matura");
                var staraMatura = matury.Single(x => x.Nazwa == "Stara matura");

                var nowaLista = new List<PrzedmiotRodzajPoziom>();
                var staraLista = new List<PrzedmiotRodzajPoziom>();

                var starePrzedmiotyDopuszczalne = new HashSet<string>
        {
            "Język polski",
            "Matematyka",
            "Język angielski",
            "Język niemiecki",
            "Język francuski",
            "Język hiszpański",
            "Język rosyjski",
            "Biologia",
            "Chemia",
            "Fizyka",
            "Geografia",
            "Historia",
            "Historia muzyki",
            "Historia sztuki",
            "Informatyka",
            "Wiedza o społeczeństwie",
            "Filozofia"
        };

                foreach (var cfg in konfiguracje)
                {
                    // NOWA MATURA
                    nowaLista.Add(cfg);

                    // STARA MATURA
                    if (starePrzedmiotyDopuszczalne.Contains(cfg.Przedmiot!.Nazwa))
                    {
                        staraLista.Add(cfg);
                    }
                }

                foreach (var cfg in nowaLista)
                {
                    context.MaturaPrzedmioty.Add(new MaturaPrzedmiot
                    {
                        MaturaId = nowaMatura.IdMatura,
                        PrzedmiotRodzajPoziomId = cfg.IdPrzedmiotRodzajPoziom
                    });
                }

                foreach (var cfg in staraLista)
                {
                    context.MaturaPrzedmioty.Add(new MaturaPrzedmiot
                    {
                        MaturaId = staraMatura.IdMatura,
                        PrzedmiotRodzajPoziomId = cfg.IdPrzedmiotRodzajPoziom
                    });
                }

                await context.SaveChangesAsync();
            }

            // ======================
            // PRZYKŁADOWE ALGORYTMY
            // tylko techniczne placeholdery, bez konfiguracji liczenia
            // ======================
            if (!await context.Algorytmy.AnyAsync())
            {
                context.Algorytmy.AddRange(
                    new Algorytm { Nazwa = "Algorytm testowy 1" },
                    new Algorytm { Nazwa = "Algorytm testowy 2" }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // PRZYKŁADOWE KIERUNKI
            // ======================
            if (!await context.Kierunki.AnyAsync())
            {
                var trybStacjonarne = await context.KierunekTryby
                    .SingleAsync(x => x.Nazwa == "Stacjonarne");

                var trybNiestacjonarne = await context.KierunekTryby
                    .SingleAsync(x => x.Nazwa == "Niestacjonarne");

                var rodzaj1Stopnia = await context.KierunekRodzaje
                    .SingleAsync(x => x.Nazwa == "I stopnia");

                var rodzajJednolite = await context.KierunekRodzaje
                    .SingleAsync(x => x.Nazwa == "Jednolite mgr");

                var algorytm1 = await context.Algorytmy
                    .OrderBy(x => x.IdAlgorytm)
                    .FirstAsync();

                var algorytm2 = await context.Algorytmy
                    .OrderBy(x => x.IdAlgorytm)
                    .Skip(1)
                    .FirstAsync();

                context.Kierunki.AddRange(
                    new Kierunek
                    {
                        Nazwa = "Informatyka",
                        TrybId = trybStacjonarne.IdTryb,
                        RodzajId = rodzaj1Stopnia.IdRodzaj,
                        MinPrzyjetych = 10,
                        MaxPrzyjetych = 120,
                        MaxListaRezerwowa = 50,
                        AlgorytmId = algorytm1.IdAlgorytm
                    },
                    new Kierunek
                    {
                        Nazwa = "Automatyka i robotyka",
                        TrybId = trybStacjonarne.IdTryb,
                        RodzajId = rodzaj1Stopnia.IdRodzaj,
                        MinPrzyjetych = 10,
                        MaxPrzyjetych = 90,
                        MaxListaRezerwowa = 40,
                        AlgorytmId = algorytm1.IdAlgorytm
                    },
                    new Kierunek
                    {
                        Nazwa = "Lekarski",
                        TrybId = trybStacjonarne.IdTryb,
                        RodzajId = rodzajJednolite.IdRodzaj,
                        MinPrzyjetych = 10,
                        MaxPrzyjetych = 60,
                        MaxListaRezerwowa = 30,
                        AlgorytmId = algorytm2.IdAlgorytm
                    },
                    new Kierunek
                    {
                        Nazwa = "Administracja",
                        TrybId = trybNiestacjonarne.IdTryb,
                        RodzajId = rodzaj1Stopnia.IdRodzaj,
                        MinPrzyjetych = 10,
                        MaxPrzyjetych = 80,
                        MaxListaRezerwowa = 30,
                        AlgorytmId = algorytm1.IdAlgorytm
                    }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // PRZYKŁADOWI KANDYDACI
            // ======================
            if (!await context.Kandydaci.AnyAsync())
            {
                context.Kandydaci.AddRange(
                    new Kandydat
                    {
                        Imie = "Jan",
                        Nazwisko = "Kowalski",
                        Telefon = "500100200",
                        Email = "jan.kowalski@example.com"
                    },
                    new Kandydat
                    {
                        Imie = "Anna",
                        Nazwisko = "Nowak",
                        Telefon = "500100201",
                        Email = "anna.nowak@example.com"
                    },
                    new Kandydat
                    {
                        Imie = "Piotr",
                        Nazwisko = "Wiśniewski",
                        Telefon = "500100202",
                        Email = "piotr.wisniewski@example.com"
                    },
                    new Kandydat
                    {
                        Imie = "Katarzyna",
                        Nazwisko = "Wójcik",
                        Telefon = "500100203",
                        Email = "katarzyna.wojcik@example.com"
                    },
                    new Kandydat
                    {
                        Imie = "Michał",
                        Nazwisko = "Kamiński",
                        Telefon = "500100204",
                        Email = "michal.kaminski@example.com"
                    }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // PRZYKŁADOWE DYPLOMY
            // ======================
            if (!await context.KandydatDyplomy.AnyAsync())
            {
                var kandydaci = await context.Kandydaci
                    .OrderBy(x => x.IdKandydat)
                    .ToListAsync();

                var nowaMatura = await context.Matury.SingleAsync(x => x.Nazwa == "Nowa matura");
                var staraMatura = await context.Matury.SingleAsync(x => x.Nazwa == "Stara matura");

                for (int i = 0; i < kandydaci.Count; i++)
                {
                    var kandydat = kandydaci[i];
                    var maturaId = i == 4 ? staraMatura.IdMatura : nowaMatura.IdMatura;

                    context.KandydatDyplomy.Add(new KandydatDyplom
                    {
                        KandydatId = kandydat.IdKandydat,
                        MaturaId = maturaId,
                        Numer = $"DYP-{2024}-{kandydat.IdKandydat:D4}",
                        Rok = 2024
                    });
                }

                await context.SaveChangesAsync();
            }

            // ======================
            // PRZYKŁADOWE WYNIKI MATUR
            // ======================
            if (!await context.KandydatDyplomPrzedmioty.AnyAsync())
            {
                var dyplomy = await context.KandydatDyplomy
                    .Include(x => x.Matura)
                    .OrderBy(x => x.IdKandydatDyplom)
                    .ToListAsync();

                var konfiguracje = await context.PrzedmiotRodzajPoziomy
                    .Include(x => x.Przedmiot)
                    .Include(x => x.PrzedmiotRodzaj)
                    .Include(x => x.PrzedmiotPoziom)
                    .ToListAsync();

                int GetCfgId(string przedmiot, string rodzaj, string poziom)
                {
                    return konfiguracje.Single(x =>
                        x.Przedmiot!.Nazwa == przedmiot &&
                        x.PrzedmiotRodzaj!.Nazwa == rodzaj &&
                        x.PrzedmiotPoziom!.Nazwa == poziom).IdPrzedmiotRodzajPoziom;
                }

                foreach (var dyplom in dyplomy)
                {
                    if (dyplom.Matura!.Nazwa == "Nowa matura")
                    {
                        var wpisy = new List<(string przedmiot, string rodzaj, string poziom, decimal punkty)>
                {
                    ("Język polski", "Pisemny", "Podstawowa", 72m),
                    ("Matematyka", "Pisemny", "Podstawowa", 88m),
                    ("Język angielski", "Pisemny", "Podstawowa", 91m),
                    ("Język polski", "Ustny", "Podstawowa", 80m),
                    ("Język angielski", "Ustny", "Podstawowa", 90m),
                    ("Matematyka", "Pisemny", "Rozszerzenie", 76m),
                    ("Informatyka", "Pisemny", "Rozszerzenie", 84m),
                    ("Fizyka", "Pisemny", "Rozszerzenie", 69m)
                };

                        // lekkie różnicowanie kandydatów
                        decimal bonus = dyplom.IdKandydatDyplom switch
                        {
                            1 => 0m,
                            2 => 5m,
                            3 => -4m,
                            4 => 8m,
                            _ => 0m
                        };

                        foreach (var w in wpisy)
                        {
                            var wartosc = w.punkty + bonus;
                            if (wartosc < 0m) wartosc = 0m;
                            if (wartosc > 100m) wartosc = 100m;

                            context.KandydatDyplomPrzedmioty.Add(new KandydatDyplomPrzedmiot
                            {
                                KandydatDyplomId = dyplom.IdKandydatDyplom,
                                PrzedmiotRodzajPoziomId = GetCfgId(w.przedmiot, w.rodzaj, w.poziom),
                                Punkty = wartosc
                            });
                        }
                    }
                    else
                    {
                        var wpisyStare = new List<(string przedmiot, string rodzaj, string poziom, decimal punkty)>
                {
                    ("Język polski", "Pisemny", "Podstawowa", 4.5m),
                    ("Matematyka", "Pisemny", "Podstawowa", 5.0m),
                    ("Język angielski", "Pisemny", "Podstawowa", 4.0m),
                    ("Historia", "Pisemny", "Rozszerzenie", 5.5m),
                    ("Wiedza o społeczeństwie", "Pisemny", "Rozszerzenie", 4.5m)
                };

                        foreach (var w in wpisyStare)
                        {
                            context.KandydatDyplomPrzedmioty.Add(new KandydatDyplomPrzedmiot
                            {
                                KandydatDyplomId = dyplom.IdKandydatDyplom,
                                PrzedmiotRodzajPoziomId = GetCfgId(w.przedmiot, w.rodzaj, w.poziom),
                                Punkty = w.punkty
                            });
                        }
                    }
                }

                await context.SaveChangesAsync();
            }

            // ======================
            // PRZYKŁADOWE ZGŁOSZENIA KANDYDATÓW NA KIERUNKI
            // bez liczenia punktów przez algorytm, tylko techniczne dane testowe
            // ======================
            if (!await context.KandydatKierunki.AnyAsync())
            {
                var kandydaci = await context.Kandydaci.OrderBy(x => x.IdKandydat).ToListAsync();
                var kierunki = await context.Kierunki.OrderBy(x => x.IdKierunek).ToListAsync();
                var statusNiezakwalifikowany = await context.Statusy.SingleAsync(x => x.Nazwa == "Niezakwalifikowany");

                foreach (var kandydat in kandydaci)
                {
                    foreach (var kierunek in kierunki.Take(2))
                    {
                        context.KandydatKierunki.Add(new KandydatKierunek
                        {
                            KandydatId = kandydat.IdKandydat,
                            KierunekId = kierunek.IdKierunek,
                            Punkty = null,
                            Ranking = null,
                            StatusId = null
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}