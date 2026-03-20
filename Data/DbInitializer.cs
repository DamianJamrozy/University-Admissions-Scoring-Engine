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
            if (!await context.Statusy.AnyAsync())
            {
                context.Statusy.AddRange(
                    new Status { Nazwa = "Przyjęty" },
                    new Status { Nazwa = "Lista rezerwowa" },
                    new Status { Nazwa = "Niezakwalifikowany" }
                );
            }

            if (!await context.PrzedmiotRodzaje.AnyAsync())
            {
                context.PrzedmiotRodzaje.AddRange(
                    new PrzedmiotRodzaj { Nazwa = "Ustny" },
                    new PrzedmiotRodzaj { Nazwa = "Pisemny" }
                );
            }

            if (!await context.PrzedmiotPoziomy.AnyAsync())
            {
                context.PrzedmiotPoziomy.AddRange(
                    new PrzedmiotPoziom { Nazwa = "Podstawowa" },
                    new PrzedmiotPoziom { Nazwa = "Rozszerzenie" }
                );
            }

            if (!await context.KierunekTryby.AnyAsync())
            {
                context.KierunekTryby.AddRange(
                    new KierunekTryb { Nazwa = "Stacjonarne" },
                    new KierunekTryb { Nazwa = "Niestacjonarne" }
                );
            }

            if (!await context.KierunekRodzaje.AnyAsync())
            {
                context.KierunekRodzaje.AddRange(
                    new KierunekRodzaj { Nazwa = "I stopnia" },
                    new KierunekRodzaj { Nazwa = "II stopnia" },
                    new KierunekRodzaj { Nazwa = "Jednolite mgr" }
                );
            }

            if (!await context.AlgorytmOperacje.AnyAsync())
            {
                context.AlgorytmOperacje.AddRange(
                    new AlgorytmOperacja { Nazwa = "SUMA" },
                    new AlgorytmOperacja { Nazwa = "LUB" }
                );
            }

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
            }

            await context.SaveChangesAsync();
        }
    }
}