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

    public class MeteringCodeService : IMeteringCodeService
    {
        private static List<MeteringCode> meteringCodes;
        private readonly Dictionary<MeteringCodeColumnBy, Func<MeteringCode, object>> userActivityClause =
                  new Dictionary<MeteringCodeColumnBy, Func<MeteringCode, object>>
                    {
                        {MeteringCodeColumnBy.Zählpunktbezeichner, c => c.Zählpunktbezeichner},
                        {MeteringCodeColumnBy.Kurzbezeichnung, c => c.Kurzbezeichnung},
                        {MeteringCodeColumnBy.Anlagentyp, c => c.Anlagentyp},
                        {MeteringCodeColumnBy.Strasse, c => c.Strasse},
                        {MeteringCodeColumnBy.PLZ, c => c.PLZ},
                        {MeteringCodeColumnBy.Ort, c => c.Ort},
                        {MeteringCodeColumnBy.Datenversand, c => c.Datenversand},
                        {MeteringCodeColumnBy.Zählverfahren, c => c.Zählverfahren},
                        {MeteringCodeColumnBy.Messung, c => c.Messung}
                    };
        static MeteringCodeService()
        {
            meteringCodes = new List<MeteringCode>();
            for (int i = 1; i <= 15; i++ )
                meteringCodes.Add(new MeteringCode
                {
                    CustomerId = CustomerService.customers[(i % 5) + 1].Id,
                    Anlagentyp = i + "Test Anlagentyp",
                    Datenversand = i + " Test Datenversand",
                    Entnahme = i + "Test Entnahme",
                    Id = Guid.NewGuid(),
                    Kundenrückmeldung = i + "Test Kundenrückmeldung",
                    Kurzbezeichnung = i + "Test Kurzbezeichnung",
                    Messung = i + "Test Messung",
                    Ort = i + "TEST ORT",
                    PLZ = i + "TEST PLZ",
                    Strasse = i + "TEST Strasse",
                    Zählpunktbezeichner = i + "Test Zählpunktbezeichner",
                    Zählverfahren = i + "Test Zählverfahren",
                    MeteringCodeThresholds = new List<MeteringCodeThreshold> { new MeteringCodeThreshold
                    {
                        GrenzwertBezeichner = "Test",
                        GultingAb = DateTime.Now,
                        Id = Guid.NewGuid(),
                        MaximaGlobal = "0",
                        MinimaGlobal = "0",
                        MaximaSommer = "0",
                        MinimaSommer = "0",
                        MaximaWinter = "0",
                        MinimaWinter = "0"
                    }, new MeteringCodeThreshold
                    {
                        GrenzwertBezeichner = "Test",
                        GultingAb = DateTime.Now,
                        Id= Guid.NewGuid(),
                        MaximaGlobal = "0",
                        MinimaGlobal = "0",
                        MaximaSommer = "0",
                        MinimaSommer = "0",
                        MaximaWinter = "0",
                        MinimaWinter = "0"
                    }}
                });
        }

        public MeteringCodeResponse GetMeteringCodesByCustomerId(MeteringCodeSearchRequest searchRequest)
        {
            int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
            int toRow = searchRequest.PageSize;

            Func<MeteringCode, bool> expression =
                s => (s.CustomerId == searchRequest.CustomerId && (string.IsNullOrEmpty(searchRequest.Param) || s.Anlagentyp.Contains(searchRequest.Param) || s.Kundenrückmeldung.Contains(searchRequest.Param) || s.Kurzbezeichnung.Contains(searchRequest.Param) || s.Messung.Contains(searchRequest.Param)));

            IEnumerable<MeteringCode> oList =
            searchRequest.IsAsc ?
            meteringCodes.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
            meteringCodes.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();
            return new MeteringCodeResponse { MeteringCodes= oList, TotalCount = meteringCodes.Where(expression).ToList().Count };
   
        }

        public MeteringCode GetMeteringCodeById(Guid id)
        {
            return meteringCodes.Find(x => x.Id == id);
        }
    }
}
