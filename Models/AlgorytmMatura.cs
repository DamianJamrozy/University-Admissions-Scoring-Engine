using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class AlgorytmMatura
    {
        [Key]
        public int IdAlgorytmMatura { get; set; }

        public int AlgorytmId { get; set; }
        public int MaturaId { get; set; }

        public Algorytm? Algorytm { get; set; }
        public Matura? Matura { get; set; }

        public ICollection<AlgorytmGrupa> Grupy { get; set; } = new List<AlgorytmGrupa>();
    }
}