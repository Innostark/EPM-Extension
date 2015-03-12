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
            meteringPoints = crmService.GetBeitreiberMetringPoints(searchRequest.CustomerId, searchRequest.BetrieberId);
            int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
            bool searchSpecified = !string.IsNullOrEmpty(searchRequest.Param);
            int toRow = searchRequest.PageSize;

            Func<MeteringPoint, bool> expression =
                s => (!searchSpecified && s.CrmAccountId == searchRequest.CustomerId
                     || (s.CrmAccountId == searchRequest.CustomerId && s.Kurzbezeichnung.IndexOf(searchRequest.Param, StringComparison.OrdinalIgnoreCase ) >=0 ));

            IEnumerable<MeteringPoint> oList =
            searchRequest.IsAsc ?
            meteringPoints.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
            meteringPoints.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();
            return new MeteringPointResponse { MeteringPoints= oList, TotalCount = meteringPoints.Where(expression).ToList().Count };
   
        }

        public MeteringPoint GetMeteringPointsById(Guid id)
        {
            return meteringPoints.Find(x => x.Id == id);
        }
    }
}
