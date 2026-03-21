using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmElementCreateViewModel
    {
        [Required]
        public int AlgorytmGrupaId { get; set; }

        [Required]
        public int PrzedmiotRodzajPoziomId { get; set; }

        [Required]
        public decimal Liczba { get; set; }
    }
}