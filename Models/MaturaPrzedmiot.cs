using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class MaturaPrzedmiot
    {
        [Key]
        public int IdMaturaPrzedmiot { get; set; }

        public int MaturaId { get; set; }
        public int PrzedmiotRodzajPoziomId { get; set; }

        public Matura? Matura { get; set; }
        public PrzedmiotRodzajPoziom? PrzedmiotRodzajPoziom { get; set; }
    }
}