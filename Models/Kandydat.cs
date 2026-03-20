using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class Kandydat
    {
        [Key]
        public int IdKandydat { get; set; }

        [Required]
        [MaxLength(100)]
        public string Imie { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Nazwisko { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Telefon { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        public ICollection<KandydatDyplom> KandydatDyplomy { get; set; } = new List<KandydatDyplom>();
        public ICollection<KandydatKierunek> KandydatKierunki { get; set; } = new List<KandydatKierunek>();
    }
}