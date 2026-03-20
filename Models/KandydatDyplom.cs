using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class KandydatDyplom
    {
        [Key]
        public int IdKandydatDyplom { get; set; }

        public int KandydatId { get; set; }
        public int MaturaId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Numer { get; set; } = string.Empty;

        public int Rok { get; set; }

        public Kandydat? Kandydat { get; set; }
        public Matura? Matura { get; set; }

        public ICollection<KandydatDyplomPrzedmiot> KandydatDyplomPrzedmioty { get; set; } = new List<KandydatDyplomPrzedmiot>();
    }
}