using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPM.Extension.Model;
using EPM.Extension.Model.RequestModels;

namespace EPM.Extension.Web.Models
{
    public class MeteringCodeViewModel
    {
        public IEnumerable<MeteringCode> data { get; set; }
       
        public MeteringCodeSearchRequest SearchRequest { get; set; }
        
        public int recordsTotal;

        public int recordsFiltered;
    }

  
}
