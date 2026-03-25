using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class PrzedmiotWariantFormViewModel
    {
        public int? IdPrzedmiotRodzajPoziom { get; set; }

        [Required]
        [Display(Name = "Przedmiot")]
        public int PrzedmiotId { get; set; }

        [Required]
        [Display(Name = "Tryb")]
        public int PrzedmiotRodzajId { get; set; }

        [Required]
        [Display(Name = "Poziom")]
        public int PrzedmiotPoziomId { get; set; }

        public List<SelectListItem> Przedmioty { get; set; } = new();
        public List<SelectListItem> Rodzaje { get; set; } = new();
        public List<SelectListItem> Poziomy { get; set; } = new();
    }
}