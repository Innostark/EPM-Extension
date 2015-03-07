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
        public string MinimaGlobal { get; set; }
        public string MaximaGlobal { get; set; }
        public string MinimaSommer { get; set; }
        public string MaximaSommer { get; set; }
        public string MinimaWinter { get; set; }
        public string MaximaWinter { get; set; }

    }
}
