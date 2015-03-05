using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model
{
    public class CrmAccount
    {
        public Guid Id { get; set; }
        [Display(Name = "Name", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]  
        public string Kunde { get; set; }

        [Display(Name = "Number", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string Kundennummer { get; set; }

        [Display(Name = "Address", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string Strasse { get; set; }

        [Display(Name = "ZipCode", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string Plz { get; set; }

        [Display(Name = "City", ResourceType = typeof(EPM.Extension.Model.Resources.CustomerResource))]
        public string Ort { get; set; }

        public IEnumerable<MeteringCode> MeteringCodes { get; set; } 
    }
}
