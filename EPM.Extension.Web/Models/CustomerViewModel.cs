using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPM.Extension.Model;
using EPM.Extension.Model.RequestModels;

namespace EPM.Extension.Web.Models
{
    public class CustomerViewModel
    {
        public IEnumerable<CrmAccount> data { get; set; }
        public IEnumerable<MeteringPoint> MeteringdCodes { get; set; }
        public CustomerSearchRequest SearchRequest { get; set; }
        public MeteringPointSearchRequest MeteringCodeSearchRequest { get; set; }

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
