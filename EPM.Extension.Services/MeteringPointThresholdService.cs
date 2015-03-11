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

    public class MeteringPointThresholdService : IMeteringPointThresholdService
    {
        static MeteringPointThresholdService()
        {
            DynamicsCrmService crmService = new DynamicsCrmService();
        }

        public MeteringPointThreshold GetMeteringPointThresholdById(Guid id)
        {
            DynamicsCrmService dynamicsCrmService = new DynamicsCrmService();

            return dynamicsCrmService.GetThresholdById(id);

            //return new MeteringPointThreshold
            //{
            //    Id = id,
            //    GrenzwertBezeichner = "test",
            //    GultingAb = DateTime.Now,
            //    MaximaGlobal = "10",
            //    MinimaGlobal = "20",
            //    MaximaSommer = "30",
            //    MinimaSommer = "40",
            //    MaximaWinter = "50",
            //    MinimaWinter = "60"
            //};
        }

        public void UpdateMeteringThreshold(MeteringPointThreshold model)
        {
            DynamicsCrmService dynamicsCrmService = new DynamicsCrmService();
            dynamicsCrmService.UpdateMeteringPointThreshold(model);
        }
    }
}
