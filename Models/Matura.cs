using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Admissions_Scoring_Engine.Models
{
    public class Matura
    {
        [Key]
        public int IdMatura { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal SkalaOd { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SkalaDo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SkalaUnit { get; set; }

        public ICollection<MaturaPrzedmiot> MaturaPrzedmioty { get; set; } = new List<MaturaPrzedmiot>();
        public ICollection<KandydatDyplom> KandydatDyplomy { get; set; } = new List<KandydatDyplom>();
    }
}