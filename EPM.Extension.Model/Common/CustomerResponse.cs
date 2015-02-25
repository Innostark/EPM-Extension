using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model.Common
{
    public class CustomerResponse
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerResponse()
        {
            Customers = new List<Customer>();
            
        }

        /// <summary>
        /// Activities
        /// </summary>
        public IEnumerable<Customer> Customers { get; set; }

       

        /// <summary>
        /// Total Count
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total Count
        /// </summary>
        public int UserTotalCount { get; set; }
    }
}
