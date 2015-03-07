using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPM.Extension.Model;
using EPM.Extension.Model.RequestModels;

namespace EPM.Extension.Web.Models
{
    public class MeteringPointViewModel
    {
        public IEnumerable<MeteringPoint> data { get; set; }
       
        public MeteringPointSearchRequest SearchRequest { get; set; }
        
        public int recordsTotal;

        public int recordsFiltered;
    }

  
}
