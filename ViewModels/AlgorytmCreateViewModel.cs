using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmCreateViewModel
    {
        [Required]
        [MaxLength(150)]
        public string Nazwa { get; set; } = string.Empty;
    }
}