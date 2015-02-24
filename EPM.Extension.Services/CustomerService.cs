using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Services
{
    using Interfaces;

    public class CustomerService: ICustomerService
    {
        public IEnumerable<Model.Customer> GetAllCustomers()
        {
            throw new NotImplementedException();
        }
    }
}
