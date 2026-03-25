using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class KierunekFormViewModel
    {
        public int? IdKierunek { get; set; }

        [Required(ErrorMessage = "Nazwa kierunku jest wymagana.")]
        [Display(Name = "Nazwa kierunku")]
        public string Nazwa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wybierz tryb.")]
        [Display(Name = "Tryb")]
        public int TrybId { get; set; }

        [Required(ErrorMessage = "Wybierz rodzaj.")]
        [Display(Name = "Rodzaj")]
        public int RodzajId { get; set; }

        [Required(ErrorMessage = "Podaj minimalną liczbę przyjętych.")]
        [Range(0, int.MaxValue, ErrorMessage = "Wartość musi być większa lub równa 0.")]
        [Display(Name = "Minimalna liczba przyjętych")]
        public int MinPrzyjetych { get; set; }

        [Required(ErrorMessage = "Podaj maksymalną liczbę przyjętych.")]
        [Range(0, int.MaxValue, ErrorMessage = "Wartość musi być większa lub równa 0.")]
        [Display(Name = "Maksymalna liczba przyjętych")]
        public int MaxPrzyjetych { get; set; }

        [Required(ErrorMessage = "Podaj maksymalną liczbę osób na liście rezerwowej.")]
        [Range(0, int.MaxValue, ErrorMessage = "Wartość musi być większa lub równa 0.")]
        [Display(Name = "Maksymalna lista rezerwowa")]
        public int MaxListaRezerwowa { get; set; }

        [Display(Name = "Algorytm")]
        public int? AlgorytmId { get; set; }

        public List<SelectListItem> Tryby { get; set; } = new();
        public List<SelectListItem> Rodzaje { get; set; } = new();
        public List<SelectListItem> Algorytmy { get; set; } = new();
    }
}