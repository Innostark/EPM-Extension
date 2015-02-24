using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Model
{
    public class Customer
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public IEnumerable<MeteringCode> MeteringCodes { get; set; } 
    }
}
