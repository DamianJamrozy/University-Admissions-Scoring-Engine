using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class MaturaPrzedmiotyViewModel
    {
        public Matura Matura { get; set; } = null!;
        public List<PrzedmiotRodzajPoziom> WszystkieWarianty { get; set; } = new();
        public List<int> WybraneWariantyIds { get; set; } = new();
    }
}