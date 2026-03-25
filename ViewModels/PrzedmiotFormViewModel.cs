using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class PrzedmiotFormViewModel
    {
        public int? IdPrzedmiot { get; set; }

        [Required(ErrorMessage = "Nazwa przedmiotu jest wymagana.")]
        [Display(Name = "Nazwa przedmiotu")]
        public string Nazwa { get; set; } = string.Empty;
    }
}