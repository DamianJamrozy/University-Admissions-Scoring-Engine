using University_Admissions_Scoring_Engine.Models;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmGroupTreeNodeViewModel
    {
        public AlgorytmGrupa CurrentGroup { get; set; } = null!;
        public List<AlgorytmGrupa> AllGroups { get; set; } = new();
        public List<AlgorytmLicz> AllElements { get; set; } = new();
        public int AlgorytmId { get; set; }
        public bool IsRoot { get; set; }
    }
}