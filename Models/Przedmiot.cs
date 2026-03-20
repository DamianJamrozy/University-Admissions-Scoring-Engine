using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class Przedmiot
    {
        [Key]
        public int IdPrzedmiot { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; } = string.Empty;

        public ICollection<PrzedmiotRodzajPoziom> PrzedmiotRodzajPoziomy { get; set; } = new List<PrzedmiotRodzajPoziom>();
    }
}