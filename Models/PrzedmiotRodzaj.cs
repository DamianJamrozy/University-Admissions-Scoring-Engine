using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class PrzedmiotRodzaj
    {
        [Key]
        public int IdPrzedmiotRodzaj { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwa { get; set; } = string.Empty;

        public ICollection<PrzedmiotRodzajPoziom> PrzedmiotRodzajPoziomy { get; set; } = new List<PrzedmiotRodzajPoziom>();
    }
}