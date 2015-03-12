using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPM.Extension.Model.RequestModels;

namespace EPM.Extension.Services
{
    using EPM.Extension.Model;
    using EPM.Extension.Model.Common;
    using Interfaces;
    using EPM.Extension.Services.DynamicsCRM;

    public class MeteringPointService : IMeteringPointService
    {
        private static List<MeteringPoint> meteringPoints;
        private DynamicsCrmService crmService;
        private readonly Dictionary<MeteringPointColumnBy, Func<MeteringPoint, object>> userActivityClause =
                  new Dictionary<MeteringPointColumnBy, Func<MeteringPoint, object>>
                    {
                        {MeteringPointColumnBy.Zählpunktbezeichner, c => c.Zählpunktbezeichner},
                        {MeteringPointColumnBy.Kurzbezeichnung, c => c.Kurzbezeichnung},
                        {MeteringPointColumnBy.Anlagentyp, c => c.Anlagentyp},
                        {MeteringPointColumnBy.Strasse, c => c.Strasse},
                        {MeteringPointColumnBy.PLZ, c => c.PLZ},
                        {MeteringPointColumnBy.Ort, c => c.Ort},
                        {MeteringPointColumnBy.Datenversand, c => c.DatenversandAktiv},
                        {MeteringPointColumnBy.Zählverfahren, c => c.ZählverfahrenValue},
                        {MeteringPointColumnBy.Messung, c => c.UMessungValue}
                    };
        public MeteringPointService()
        {
            crmService  = new DynamicsCrmService();
        }

        public MeteringPointResponse GetMeteringPointsByCustomerId(MeteringPointSearchRequest searchRequest)
        {
            return crmService.GetBeitreiberMetringPoints(searchRequest);
        }

        public MeteringPoint GetMeteringPointsById(Guid id)
        {
            return meteringPoints.Find(x => x.Id == id);
        }
    }
}
