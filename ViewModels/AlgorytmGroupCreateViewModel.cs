using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.ViewModels
{
    public class AlgorytmGroupCreateViewModel
    {
        [Required]
        public int AlgorytmId { get; set; }

        public int? RodzicId { get; set; }

        [Required]
        public int AlgorytmOperacjaId { get; set; }
    }
}