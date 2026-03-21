using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Admissions_Scoring_Engine.Models
{
    public class AlgorytmGrupa
    {
        [Key]
        public int IdAlgorytmGrupa { get; set; }

        public int AlgorytmMaturaId { get; set; }

        public int? RodzicId { get; set; }

        public int AlgorytmOperacjaId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinimalnePunkty { get; set; }

        public AlgorytmMatura? AlgorytmMatura { get; set; }
        public AlgorytmGrupa? Rodzic { get; set; }
        public AlgorytmOperacja? AlgorytmOperacja { get; set; }

        public ICollection<AlgorytmLicz> Elementy { get; set; } = new List<AlgorytmLicz>();
    }
}