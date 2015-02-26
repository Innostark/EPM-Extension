using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model
{
    public class MeteringCode
    {
        public int Id { get; set; }
        public string Zählpunktbezeichner { get; set; }
        public string Kurzbezeichnung { get; set; }
        public string Anlagentyp { get; set; }
        public string Strasse { get; set; }
        public string PLZ { get; set; }
        public string Ort { get; set; }
        public string Datenversand { get; set; }
        public string Zählverfahren { get; set; }
        public string Messung { get; set; }
        public string Entnahme { get; set; }
        public string Kundenrückmeldung { get; set; }
        public int CustomerId { get; set; }

    }
}
