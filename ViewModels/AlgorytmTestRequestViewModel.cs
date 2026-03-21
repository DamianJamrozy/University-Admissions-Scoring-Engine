namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmTestRequestViewModel
    {
        public int AlgorytmId { get; set; }
        public int MaturaId { get; set; }
        public List<AlgorytmTestInputViewModel> Inputs { get; set; } = new();
    }

    public class AlgorytmTestInputViewModel
    {
        public int PrzedmiotRodzajPoziomId { get; set; }
        public decimal Punkty { get; set; }
    }
}