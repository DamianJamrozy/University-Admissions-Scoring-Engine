using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class Algorytm
    {
        [Key]
        public int IdAlgorytm { get; set; }

        public string Nazwa { get; set; } = string.Empty;

        public ICollection<AlgorytmMatura> AlgorytmyMatur { get; set; } = new List<AlgorytmMatura>();
    }
}