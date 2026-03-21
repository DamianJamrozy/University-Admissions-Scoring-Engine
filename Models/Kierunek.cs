using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class Kierunek
    {
        [Key]
        public int IdKierunek { get; set; }

        public string Nazwa { get; set; } = string.Empty;

        public int TrybId { get; set; }
        public int RodzajId { get; set; }

        public int MinPrzyjetych { get; set; }
        public int MaxPrzyjetych { get; set; }
        public int MaxListaRezerwowa { get; set; }

        public int? AlgorytmId { get; set; }

        public KierunekTryb? Tryb { get; set; }
        public KierunekRodzaj? Rodzaj { get; set; }
        public Algorytm? Algorytm { get; set; }

        public ICollection<KandydatKierunek> KandydatKierunki { get; set; } = new List<KandydatKierunek>();
    }
}