using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPM.Extension.Model.Common;
using EPM.Extension.Model.RequestModels;

namespace EPM.Extension.Interfaces
{
    using Model;

    public interface IMeteringPointService
    {
        MeteringPointResponse GetMeteringPointsByCustomerId(MeteringPointSearchRequest request);
        MeteringPoint GetMeteringPointsById(Guid id);
        MeteringPoint GetMeteringPointsByCode(string id);
        string GetReportSelectedValue(MeteringPoint mp, string defaultId,  string aktivId, string inAktivId);
        void UpdateMeteringPointSpecificationsAndThreashold(MeteringPoint meteringPoint);
    }
}
