using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmGroupTreeNodeViewModel
    {
        public AlgorytmGrupa CurrentGroup { get; set; } = null!;
        public List<AlgorytmGrupa> AllGroups { get; set; } = new();
        public List<AlgorytmLicz> AllElements { get; set; } = new();
        public List<AlgorytmOperacja> Operacje { get; set; } = new();
        public List<PrzedmiotRodzajPoziom> PrzedmiotOpcje { get; set; } = new();

        public int AlgorytmId { get; set; }
        public int AlgorytmMaturaId { get; set; }

        public bool IsRoot { get; set; }

        public Dictionary<int, int> GroupNumbers { get; set; } = new();
    }
}