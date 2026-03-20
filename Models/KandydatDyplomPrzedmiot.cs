using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Admissions_Scoring_Engine.Models
{
    public class KandydatDyplomPrzedmiot
    {
        [Key]
        public int IdKandydatDyplomPrzedmiot { get; set; }

        public int KandydatDyplomId { get; set; }
        public int PrzedmiotRodzajPoziomId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Punkty { get; set; }

        public KandydatDyplom? KandydatDyplom { get; set; }
        public PrzedmiotRodzajPoziom? PrzedmiotRodzajPoziom { get; set; }
    }
}