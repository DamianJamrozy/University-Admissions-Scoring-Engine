using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class AlgorytmGrupa
    {
        [Key]
        public int IdAlgorytmGrupa { get; set; }

        public int AlgorytmId { get; set; }
        public int AlgorytmOperacjaId { get; set; }
        public int? RodzicId { get; set; }

        public Algorytm? Algorytm { get; set; }
        public AlgorytmOperacja? AlgorytmOperacja { get; set; }
        public AlgorytmGrupa? Rodzic { get; set; }

        public ICollection<AlgorytmGrupa> Dzieci { get; set; } = new List<AlgorytmGrupa>();
        public ICollection<AlgorytmLicz> AlgorytmLicze { get; set; } = new List<AlgorytmLicz>();
    }
}