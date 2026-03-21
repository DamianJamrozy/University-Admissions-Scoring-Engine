using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Admissions_Scoring_Engine.Models
{
    public class AlgorytmLicz
    {
        [Key]
        public int IdAlgorytmLicz { get; set; }

        public int PrzedmiotRodzajPoziomId { get; set; }
        public int AlgorytmGrupaId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Liczba { get; set; }

        public bool CzyWymagany { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinimalnePunkty { get; set; }

        public PrzedmiotRodzajPoziom? PrzedmiotRodzajPoziom { get; set; }
        public AlgorytmGrupa? AlgorytmGrupa { get; set; }
    }
}