using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_Admissions_Scoring_Engine.Models
{
    public class KandydatKierunek
    {
        [Key]
        public int IdKandydatKierunek { get; set; }

        public int KandydatId { get; set; }
        public int KierunekId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Punkty { get; set; }

        public int Ranking { get; set; }
        public int StatusId { get; set; }

        public Kandydat? Kandydat { get; set; }
        public Kierunek? Kierunek { get; set; }
        public Status? Status { get; set; }
    }
}