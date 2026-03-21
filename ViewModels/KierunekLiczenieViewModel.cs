using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class KierunekLiczenieViewModel
    {
        public Kierunek Kierunek { get; set; } = null!;
        public List<KandydatKierunek> Kandydaci { get; set; } = new();
    }
}