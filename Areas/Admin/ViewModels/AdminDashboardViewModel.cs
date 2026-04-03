namespace University_Admissions_Scoring_Engine.Areas.Admin.ViewModels
{
    public class AdminDashboardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public List<AdminDashboardSectionViewModel> Sections { get; set; } = new();
    }

    public class AdminDashboardSectionViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<AdminDashboardCardViewModel> Cards { get; set; } = new();
    }

    public class AdminDashboardCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = "#";
        public string Badge { get; set; } = string.Empty;
        public string Icon { get; set; } = "bi-grid";
        public bool IsEnabled { get; set; } = true;
    }
}