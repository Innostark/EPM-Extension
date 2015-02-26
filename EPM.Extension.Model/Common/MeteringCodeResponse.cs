using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model.Common
{
    public class MeteringCodeResponse
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MeteringCodeResponse()
        {
            MeteringCodes = new List<MeteringCode>();
            
        }

        /// <summary>
        /// Activities
        /// </summary>
        public IEnumerable<MeteringCode> MeteringCodes { get; set; }

       

        /// <summary>
        /// Total Count
        /// </summary>
        public int TotalCount { get; set; }

    }
}
