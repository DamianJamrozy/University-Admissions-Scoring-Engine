using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmKierunkiViewModel
    {
        public Algorytm Algorytm { get; set; } = null!;
        public List<Kierunek> WszystkieKierunki { get; set; } = new();
        public List<int> WybraneKierunkiIds { get; set; } = new();
    }
}