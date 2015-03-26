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
            try
            {
                DynamicsCrmService dynamicsCrmService = new DynamicsCrmService();

                return dynamicsCrmService.GetThresholdById(id);
            }
            catch (Exception ex)
            {
                Trace.LogError(ex);
                throw;
            }
        }

        public void UpdateMeteringThreshold(MeteringPointThreshold model)
        {
            try
            {
                DynamicsCrmService dynamicsCrmService = new DynamicsCrmService();
                dynamicsCrmService.UpdateMeteringPointThreshold(model);
            }
            catch (Exception ex)
            {
                Trace.LogError(ex);
                throw;
            }
            
        }
    }
}
