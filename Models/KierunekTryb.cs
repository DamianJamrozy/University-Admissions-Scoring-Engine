using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class KierunekTryb
    {
        [Key]
        public int IdTryb { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; } = string.Empty;

        public ICollection<Kierunek> Kierunki { get; set; } = new List<Kierunek>();
    }
}