using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class RankingDetailsViewModel
    {
        public Kierunek Kierunek { get; set; } = null!;
        public List<KandydatKierunek> KandydaciNaKierunek { get; set; } = new();
        public int LiczbaKandydatow { get; set; }
        public bool CzyMaAlgorytm { get; set; }
        public string? NazwaAlgorytmu { get; set; }
    }
}