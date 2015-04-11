using EPM.Extension.Interfaces;
using EPM.Extension.Model;
using EPM.Extension.Model.Common;
using EPM.Extension.Model.RequestModels;
using EPM.Extension.Services.DynamicsCRM;
using EPM.Extension.Services.DynamicsCRM.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
namespace EPM.Extension.Services
{
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
            try
            {
                if (searchRequest.BetrieberId != Guid.Empty && searchRequest.CustomerId != Guid.Empty)
                {
                    return crmService.GetBeitreiberMetringPoints(searchRequest);

                }
                return new MeteringPointResponse
                {
                    MeteringPoints = new List<MeteringPoint>(),
                    TotalCount = 0
                };
            }
            catch (Exception exception)
            {
                Trace.LogError(exception);
                throw;
            }
           
        }

        public MeteringPoint GetMeteringPointsById(Guid id)
        {
            try
            {
                return crmService.GetMeteringPointById(id);

            }
            catch (Exception exception)
            {
                Trace.LogError(exception);
                throw;
            }
        }

        public MeteringPoint GetMeteringPointsByCode(string id)
        {
            try
            {
                return crmService.GetMeteringPointByCode(id);
            }
            catch (Exception exception)
            {
                Trace.LogError(exception);
                throw;
            }
           
        }

        public string GetReportSelectedValue(MeteringPoint mp,string defaultId, string aktivId, string inAktivId)
        {
            string selectedValue = defaultId;
            if (mp.MeteringCodeThresholds != null && mp.MeteringCodeThresholds.Count() > 0)
            {
                MeteringPointThreshold mpt = mp.MeteringCodeThresholds.FirstOrDefault();
                if (mpt != null)
                {
                    if (mpt.EMailBerichte == (int)MetadataGrenzwert.OpSetReport.Aktiv)
                    {
                        selectedValue = aktivId;
                    }
                    else if (mpt.EMailBerichte == (int)MetadataGrenzwert.OpSetReport.NeinAktiv)
                    {
                        selectedValue = inAktivId;
                    }
                }
            }
            return selectedValue;
        }
    }
}
