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
        CustomerResponse FindCustomers(CustomerSearchRequest request, Guid loggedInUserId);
        CustomerResponse FindBetrieber(Model.RequestModels.CustomerSearchRequest searchRequest, Guid loggedInUserId);
        CrmAccount GetCustomerById(Guid id);
        Guid GetAccount(string username, string password);
        void UpdateCustomer(CrmAccount customer);
    }
}
