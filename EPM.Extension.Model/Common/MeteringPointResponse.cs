using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model.Common
{
    public class MeteringPointResponse
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MeteringPointResponse()
        {
            MeteringPoints = new List<MeteringPoint>();
            
        }

        /// <summary>
        /// Activities
        /// </summary>
        public IEnumerable<MeteringPoint> MeteringPoints { get; set; }

       

        /// <summary>
        /// Total Count
        /// </summary>
        public int TotalCount { get; set; }

    }
}
