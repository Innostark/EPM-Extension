using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPM.Extension.Model;
using EPM.Extension.Model.RequestModels;

namespace EPM.Extension.Web.Models
{
    public class CustomerDetailViewModel
    {
        public Customer Customer { get; set; }
        public MeteringCodeSearchRequest SearchRequest { get; set; }
        
        /// <summary>
        /// Total Records in DB
        /// </summary>
        public int recordsTotalMeteringCode;
        public int recordsTotal;

        /// <summary>
        /// Total Records Filtered
        /// </summary>
        public int recordsFilteredMeteringCode;
        public int recordsFiltered;
    }

  
}
