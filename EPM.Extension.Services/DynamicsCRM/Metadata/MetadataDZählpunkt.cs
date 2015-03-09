using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Services.DynamicsCRM.Metadata
{
    public class MetadataDZählpunkt
    {
        public const string ACCOUNT = "new_account";
        public const string NAME = "new_name";
        public const string ZAHLPUNKTBEZEICHNER = "new_zpbez";
        public const string KURZEEZEICHNUNG = "new_kurzbezeichnung";
        public const string ANLAGENTYP = "new_anlagentyp";
        public const string BETREIBER  = "new_betreiber";
        public const string STRASSE = "new_strasseundhausnr";
        public const string PLZ = "new_plz";
        public const string ORT = "new_ort";
        public const string DATENVERSANDAKTIV = "new_datenversandaktiv";
        public const string ZAHLVERFAHREN = "new_zhlverfahren";
        public const string UMESSUNG = "new_spannungsebenemess";
        public const string UENTNAHME = "new_spannungsebeneentn";
        public const string KUNDENRUCKMELDUNG  = "new_rckmeldungpe";
        public const string VNB = "new_vnb";
        public const string CODE = "new_anlagentyp";
        public const string METERING_POINT_ID = "new_dzhlpunktid";

        public enum OpSetZählverfahren
        {
            RLM = 100000000,
            SLP = 100000001
        }
    }
}
