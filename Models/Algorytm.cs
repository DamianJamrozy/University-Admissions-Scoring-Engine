using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class Algorytm
    {
        [Key]
        public int IdAlgorytm { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nazwa { get; set; } = string.Empty;

        public ICollection<Kierunek> Kierunki { get; set; } = new List<Kierunek>();
        public ICollection<AlgorytmGrupa> AlgorytmGrupy { get; set; } = new List<AlgorytmGrupa>();
    }
}