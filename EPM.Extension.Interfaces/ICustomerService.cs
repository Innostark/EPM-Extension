using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPM.Extension.Model;
using EPM.Extension.Model.Common;
using EPM.Extension.Model.RequestModels;

namespace EPM.Extension.Interfaces
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAllCustomers();
        CustomerResponse FindCustomers(CustomerSearchRequest request);
        Customer GetCustomerById(int id);
        void UpdateCustomer(Customer customer);
    }
}
