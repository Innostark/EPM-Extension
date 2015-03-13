using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Services
{
    using EPM.Extension.Model.Common;
    using EPM.Extension.Services.DynamicsCRM;
    using Interfaces;
    using Model;
    using System.Linq.Expressions;
    public class CustomerService: ICustomerService
    {
        public static List<CrmAccount> customers;
        private readonly DynamicsCrmService crmService;
        private readonly Dictionary<CustomerColumnBy, Func<CrmAccount, object>> userActivityClause =
          new Dictionary<CustomerColumnBy, Func<CrmAccount, object>>
                    {
                        {CustomerColumnBy.Name, c => c.Kunde},
                        {CustomerColumnBy.Number, c => c.Kundennummer},
                        {CustomerColumnBy.Address, c => c.Strasse},
                        {CustomerColumnBy.ZipCode, c => c.Plz},
                        {CustomerColumnBy.City, c => c.Ort}
                    };
        
         public CustomerService()
        {
            crmService = new DynamicsCrmService();
        }
        

        public void UpdateCustomer(CrmAccount customer)
        {
            CrmAccount c = customers.FirstOrDefault(x => x.Id == customer.Id);
            var index = customers.IndexOf(c);
            customers.Remove(c);
            customers.Insert(index, customer);
        }

        public CrmAccount GetCustomerById(Guid id)
        {
            return crmService.GetAccountById(id);
            //return customers.FirstOrDefault(x => x.Id == id);
        }


        public CustomerResponse FindCustomers(Model.RequestModels.CustomerSearchRequest searchRequest, Guid userGuid)
        {
            return crmService.GetAccountsByUserId(searchRequest, userGuid);            
        }

        public CustomerResponse FindBetrieber(Model.RequestModels.CustomerSearchRequest searchRequest, Guid userGuid)
        {
            return crmService.GetBeitreibersByUserId(searchRequest, userGuid );
        }


        public Guid GetAccount(string username, string password)
        {
            return crmService.AuthenticateUser(username, password);
        }
    }
}
