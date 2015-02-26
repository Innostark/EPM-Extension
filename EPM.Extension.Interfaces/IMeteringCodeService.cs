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

    public interface IMeteringCodeService
    {
        MeteringCodeResponse GetMeteringCodesByCustomerId(MeteringCodeSearchRequest request);
    }
}
