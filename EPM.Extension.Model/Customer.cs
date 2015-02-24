using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model
{
    public class Customer
    {
        public int Id { get; set; }
        [Display(Name = "Name", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]  
        public string Name { get; set; }

        [Display(Name = "Number", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string Number { get; set; }

        [Display(Name = "Address", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string Address { get; set; }

        [Display(Name = "ZipCode", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string ZipCode { get; set; }

        [Display(Name = "City", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string City { get; set; }

        public IEnumerable<MeteringCode> MeteringCodes { get; set; } 
    }
}
