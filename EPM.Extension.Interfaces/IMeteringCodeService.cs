using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Interfaces
{
    using Model;

    public interface IMeteringCodeService
    {
        IList<MeteringCode> GetMeteringCodesByCustomerId();
    }
}
