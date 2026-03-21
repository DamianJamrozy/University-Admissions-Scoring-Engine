using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmDetailsViewModel
    {
        public Algorytm Algorytm { get; set; } = null!;
        public List<AlgorytmGrupa> Groups { get; set; } = new();
        public List<AlgorytmLicz> Elements { get; set; } = new();
    }
}