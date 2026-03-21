using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Matura> Matury { get; set; }
        public DbSet<Przedmiot> Przedmioty { get; set; }
        public DbSet<PrzedmiotRodzaj> PrzedmiotRodzaje { get; set; }
        public DbSet<PrzedmiotPoziom> PrzedmiotPoziomy { get; set; }
        public DbSet<PrzedmiotRodzajPoziom> PrzedmiotRodzajPoziomy { get; set; }
        public DbSet<MaturaPrzedmiot> MaturaPrzedmioty { get; set; }

        public DbSet<Kierunek> Kierunki { get; set; }
        public DbSet<KierunekRodzaj> KierunekRodzaje { get; set; }
        public DbSet<KierunekTryb> KierunekTryby { get; set; }

        public DbSet<Algorytm> Algorytmy { get; set; }
        public DbSet<AlgorytmMatura> AlgorytmyMatur { get; set; }
        public DbSet<AlgorytmGrupa> AlgorytmGrupy { get; set; }
        public DbSet<AlgorytmOperacja> AlgorytmOperacje { get; set; }
        public DbSet<AlgorytmLicz> AlgorytmLicze { get; set; }

        public DbSet<Kandydat> Kandydaci { get; set; }
        public DbSet<KandydatDyplom> KandydatDyplomy { get; set; }
        public DbSet<KandydatDyplomPrzedmiot> KandydatDyplomPrzedmioty { get; set; }
        public DbSet<KandydatKierunek> KandydatKierunki { get; set; }

        public DbSet<Status> Statusy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PrzedmiotRodzajPoziom>()
                .HasIndex(x => new { x.PrzedmiotId, x.PrzedmiotRodzajId, x.PrzedmiotPoziomId })
                .IsUnique();

            modelBuilder.Entity<MaturaPrzedmiot>()
                .HasIndex(x => new { x.MaturaId, x.PrzedmiotRodzajPoziomId })
                .IsUnique();

            modelBuilder.Entity<KandydatKierunek>()
                .HasIndex(x => new { x.KandydatId, x.KierunekId })
                .IsUnique();

            modelBuilder.Entity<KandydatDyplomPrzedmiot>()
                .HasIndex(x => new { x.KandydatDyplomId, x.PrzedmiotRodzajPoziomId })
                .IsUnique();

            modelBuilder.Entity<Kandydat>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<KandydatDyplom>()
                .HasIndex(x => x.Numer)
                .IsUnique();

            modelBuilder.Entity<AlgorytmMatura>()
                .HasIndex(x => new { x.AlgorytmId, x.MaturaId })
                .IsUnique();

            modelBuilder.Entity<AlgorytmGrupa>()
                .HasOne(x => x.Rodzic)
                .WithMany()
                .HasForeignKey(x => x.RodzicId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kierunek>()
                .HasOne(x => x.Tryb)
                .WithMany(x => x.Kierunki)
                .HasForeignKey(x => x.TrybId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kierunek>()
                .HasOne(x => x.Rodzaj)
                .WithMany(x => x.Kierunki)
                .HasForeignKey(x => x.RodzajId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kierunek>()
                .HasOne(x => x.Algorytm)
                .WithMany()
                .HasForeignKey(x => x.AlgorytmId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaturaPrzedmiot>()
                .HasOne(x => x.Matura)
                .WithMany(x => x.MaturaPrzedmioty)
                .HasForeignKey(x => x.MaturaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaturaPrzedmiot>()
                .HasOne(x => x.PrzedmiotRodzajPoziom)
                .WithMany(x => x.MaturaPrzedmioty)
                .HasForeignKey(x => x.PrzedmiotRodzajPoziomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrzedmiotRodzajPoziom>()
                .HasOne(x => x.Przedmiot)
                .WithMany(x => x.PrzedmiotRodzajPoziomy)
                .HasForeignKey(x => x.PrzedmiotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrzedmiotRodzajPoziom>()
                .HasOne(x => x.PrzedmiotRodzaj)
                .WithMany(x => x.PrzedmiotRodzajPoziomy)
                .HasForeignKey(x => x.PrzedmiotRodzajId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrzedmiotRodzajPoziom>()
                .HasOne(x => x.PrzedmiotPoziom)
                .WithMany(x => x.PrzedmiotRodzajPoziomy)
                .HasForeignKey(x => x.PrzedmiotPoziomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlgorytmMatura>()
                .HasOne(x => x.Algorytm)
                .WithMany(x => x.AlgorytmyMatur)
                .HasForeignKey(x => x.AlgorytmId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlgorytmMatura>()
                .HasOne(x => x.Matura)
                .WithMany()
                .HasForeignKey(x => x.MaturaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlgorytmGrupa>()
                .HasOne(x => x.AlgorytmMatura)
                .WithMany(x => x.Grupy)
                .HasForeignKey(x => x.AlgorytmMaturaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlgorytmGrupa>()
                .HasOne(x => x.AlgorytmOperacja)
                .WithMany()
                .HasForeignKey(x => x.AlgorytmOperacjaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlgorytmLicz>()
                .HasOne(x => x.AlgorytmGrupa)
                .WithMany(x => x.Elementy)
                .HasForeignKey(x => x.AlgorytmGrupaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlgorytmLicz>()
                .HasOne(x => x.PrzedmiotRodzajPoziom)
                .WithMany(x => x.AlgorytmLicze)
                .HasForeignKey(x => x.PrzedmiotRodzajPoziomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KandydatDyplom>()
                .HasOne(x => x.Kandydat)
                .WithMany(x => x.KandydatDyplomy)
                .HasForeignKey(x => x.KandydatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KandydatDyplom>()
                .HasOne(x => x.Matura)
                .WithMany(x => x.KandydatDyplomy)
                .HasForeignKey(x => x.MaturaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KandydatDyplomPrzedmiot>()
                .HasOne(x => x.KandydatDyplom)
                .WithMany(x => x.KandydatDyplomPrzedmioty)
                .HasForeignKey(x => x.KandydatDyplomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KandydatDyplomPrzedmiot>()
                .HasOne(x => x.PrzedmiotRodzajPoziom)
                .WithMany(x => x.KandydatDyplomPrzedmioty)
                .HasForeignKey(x => x.PrzedmiotRodzajPoziomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KandydatKierunek>()
                .HasOne(x => x.Kandydat)
                .WithMany(x => x.KandydatKierunki)
                .HasForeignKey(x => x.KandydatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KandydatKierunek>()
                .HasOne(x => x.Kierunek)
                .WithMany(x => x.KandydatKierunki)
                .HasForeignKey(x => x.KierunekId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KandydatKierunek>()
                .HasOne(x => x.Status)
                .WithMany(x => x.KandydatKierunki)
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Matura>().ToTable("Matura");
            modelBuilder.Entity<Przedmiot>().ToTable("Przedmiot");
            modelBuilder.Entity<PrzedmiotRodzaj>().ToTable("PrzedmiotRodzaj");
            modelBuilder.Entity<PrzedmiotPoziom>().ToTable("PrzedmiotPoziom");
            modelBuilder.Entity<PrzedmiotRodzajPoziom>().ToTable("Przedmiot_Rodzaj_Poziom");
            modelBuilder.Entity<MaturaPrzedmiot>().ToTable("Matura_Przedmiot");
            modelBuilder.Entity<KierunekRodzaj>().ToTable("KierunekRodzaj");
            modelBuilder.Entity<KierunekTryb>().ToTable("KierunekTryb");
            modelBuilder.Entity<Kierunek>().ToTable("Kierunek");
            modelBuilder.Entity<Algorytm>().ToTable("Algorytm");
            modelBuilder.Entity<AlgorytmMatura>().ToTable("AlgorytmMatura");
            modelBuilder.Entity<AlgorytmOperacja>().ToTable("AlgorytmOperacja");
            modelBuilder.Entity<AlgorytmGrupa>().ToTable("AlgorytmGrupa");
            modelBuilder.Entity<AlgorytmLicz>().ToTable("AlgorytmLicz");
            modelBuilder.Entity<Kandydat>().ToTable("Kandydat");
            modelBuilder.Entity<KandydatDyplom>().ToTable("KandydatDyplom");
            modelBuilder.Entity<KandydatDyplomPrzedmiot>().ToTable("KandydatDyplom_Przedmiot");
            modelBuilder.Entity<KandydatKierunek>().ToTable("Kandydat_Kierunek");
            modelBuilder.Entity<Status>().ToTable("Status");
        }
    }
}