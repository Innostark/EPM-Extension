using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model
{
    public class MeteringPointThreshold
    {
        public Guid Id { get; set; }
        public string GrenzwertBezeichner { get; set; }
        public DateTime GultingAb { get; set; }
        public bool SaisonalitatAnwenden { get; set; }
        public decimal MinimaGlobal { get; set; }
        public decimal MaximaGlobal { get; set; }
        public decimal MinimaSommer { get; set; }
        public decimal MaximaSommer { get; set; }
        public decimal MinimaWinter { get; set; }
        public decimal MaximaWinter { get; set; }
        public MeteringPointThresholdType Type { get; set; }

    }

    public enum MeteringPointThresholdType
    {
        System =1,
        User = 2
    }
}
