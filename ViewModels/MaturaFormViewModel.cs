using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class MaturaFormViewModel
    {
        public int? IdMatura { get; set; }

        [Required(ErrorMessage = "Nazwa matury jest wymagana.")]
        [Display(Name = "Nazwa matury")]
        public string Nazwa { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Skala od")]
        public decimal SkalaOd { get; set; }

        [Required]
        [Display(Name = "Skala do")]
        public decimal SkalaDo { get; set; }

        [Required]
        [Display(Name = "Skok skali")]
        public decimal SkalaUnit { get; set; }
    }
}