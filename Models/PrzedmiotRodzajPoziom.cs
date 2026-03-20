using System.ComponentModel.DataAnnotations;

namespace University_Admissions_Scoring_Engine.Models
{
    public class PrzedmiotRodzajPoziom
    {
        [Key]
        public int IdPrzedmiotRodzajPoziom { get; set; }

        public int PrzedmiotId { get; set; }
        public int PrzedmiotRodzajId { get; set; }
        public int PrzedmiotPoziomId { get; set; }

        public Przedmiot? Przedmiot { get; set; }
        public PrzedmiotRodzaj? PrzedmiotRodzaj { get; set; }
        public PrzedmiotPoziom? PrzedmiotPoziom { get; set; }

        public ICollection<MaturaPrzedmiot> MaturaPrzedmioty { get; set; } = new List<MaturaPrzedmiot>();
        public ICollection<AlgorytmLicz> AlgorytmLicze { get; set; } = new List<AlgorytmLicz>();
        public ICollection<KandydatDyplomPrzedmiot> KandydatDyplomPrzedmioty { get; set; } = new List<KandydatDyplomPrzedmiot>();
    }
}