using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class AlgorytmOperacja
    {
        [Key]
        public int IdAlgorytmOperacja { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwa { get; set; } = string.Empty;

        public ICollection<AlgorytmGrupa> AlgorytmGrupy { get; set; } = new List<AlgorytmGrupa>();
    }
}