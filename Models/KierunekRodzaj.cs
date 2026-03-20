using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class KierunekRodzaj
    {
        [Key]
        public int IdRodzaj { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; } = string.Empty;

        public ICollection<Kierunek> Kierunki { get; set; } = new List<Kierunek>();
    }
}