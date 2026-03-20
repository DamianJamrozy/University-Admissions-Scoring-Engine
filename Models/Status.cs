using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class Status
    {
        [Key]
        public int IdStatus { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; } = string.Empty;

        public ICollection<KandydatKierunek> KandydatKierunki { get; set; } = new List<KandydatKierunek>();
    }
}