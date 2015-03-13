using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPM.Extension.Model.Resources;

namespace EPM.Extension.Model
{
    public class MeteringPointThreshold
    {
        public Guid Id { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_GrenzwertBezeichner_Grenzwert_Bezeichner")]
        public string GrenzwertBezeichner { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "GultingAb")]
        public DateTime GultingAb { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_SaisonalitatAnwenden_Saisonalitat_Anwenden")]
        public bool SaisonalitatAnwenden { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_MinimaGlobal_Minima_Global")]
        public decimal MinimaGlobal { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_MaximaGlobal_Maxima_Global")]
        public decimal MaximaGlobal { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_MinimaSommer_Minima_Sommer")]
        public decimal MinimaSommer { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_MaximaSommer_Maxima_Sommer")]
        public decimal MaximaSommer { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_MinimaWinter_Minima_Winter")]
        public decimal MinimaWinter { get; set; }
        [Display(ResourceType = typeof (CustomerResource), Name = "MeteringPointThreshold_MaximaWinter_Maxima_Winter")]
        public decimal MaximaWinter { get; set; }
        public MeteringPointThresholdType Type { get; set; }

    }

    public enum MeteringPointThresholdType
    {
        System =1,
        User = 2
    }
}
