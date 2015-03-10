using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model
{
    public class MeteringPoint
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Zählpunktbezeichner { get; set; }
        public string Kurzbezeichnung { get; set; }
        public string Anlagentyp { get; set; }
        public string Strasse { get; set; }
        public string PLZ { get; set; } 
        public string Ort { get; set; }
        public string DatenversandAktiv { get; set; }
        public string Kundenrückmeldung { get; set; }
        public string Code { get; set; }
        
        #region Links
        public Guid CrmAccountId { get; set; }
        public string CrmAccountName { get; set; }
        public Guid BetreiberId { get; set; }
        public string BetreiberName { get; set; }
        public Guid VNBId { get; set; }
        [Display(Name = "VNB")]
        public string VNBName { get; set; }
        #endregion Links

        #region OptionSets
        [Display(Name = "Zählverfahren")]
        public string ZählverfahrenValue { get; set; }
        public int ZählverfahrenCode { get; set; }
        [Display(Name = "UMessung")]
        public string UMessungValue { get; set; }
        public int UMessungCode { get; set; }
        [Display(Name = "UEntnahme")]

        public string UEntnahmeValue { get; set; }
        public int UEntnahmeCode { get; set; }
        #endregion OptionSets

        public IEnumerable<MeteringPointThreshold> MeteringCodeThresholds { get; set; }

    }
}
