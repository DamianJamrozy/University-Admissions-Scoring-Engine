using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmEditorViewModel
    {
        public Algorytm Algorytm { get; set; } = null!;

        public int SelectedMaturaId { get; set; }
        public int AlgorytmMaturaId { get; set; }

        public List<Matura> Matury { get; set; } = new();

        public List<AlgorytmGrupa> Groups { get; set; } = new();
        public List<AlgorytmLicz> Elements { get; set; } = new();

        public List<AlgorytmOperacja> Operacje { get; set; } = new();
        public List<PrzedmiotRodzajPoziom> PrzedmiotOpcje { get; set; } = new();
    }
}