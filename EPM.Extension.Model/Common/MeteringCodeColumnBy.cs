using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model.Common
{
    public enum MeteringCodeColumnBy
    {
        /// <summary>
        /// Name
        /// </summary>
        Zählpunktbezeichner = 1,

        /// <summary>
        /// Points
        /// </summary>
        Kurzbezeichnung = 2,

        /// <summary>
        /// Performed Date
        /// </summary>
        Anlagentyp = 3,

        Strasse = 4,

        PLZ = 5,

        Ort= 6,
        Datenversand = 7,
        Zählverfahren = 8,
        Messung = 9, 
    }
}
