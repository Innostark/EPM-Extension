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
            try
            {
                CrmAccount c = customers.FirstOrDefault(x => x.Id == customer.Id);
                var index = customers.IndexOf(c);
                customers.Remove(c);
                customers.Insert(index, customer);
            }
            catch (Exception ex)
            {
                Trace.LogError(ex);
                throw;
            }
           
        }

        public CrmAccount GetCustomerById(Guid id)
        {
            try
            {
                return crmService.GetAccountById(id);
            }
            catch (Exception ex)
            {
                Trace.LogError(ex);
                throw;
            }
        }


        public CustomerResponse FindCustomers(Model.RequestModels.CustomerSearchRequest searchRequest, Guid userGuid)
        {
            try
            {
                return crmService.GetAccountsByUserId(searchRequest, userGuid);            
            }
            catch (Exception ex)
            {
                Trace.LogError(ex);
                throw;
            }
        }

        public CustomerResponse FindBetrieber(Model.RequestModels.CustomerSearchRequest searchRequest, Guid userGuid)
        {
            try
            {
                return crmService.GetBeitreibersByUserId(searchRequest, userGuid);
            }
            catch (Exception exception)
            {
                Trace.LogError(exception);
                throw;
            }
        }


        public Guid GetAccount(string username, string password)
        {
            try
            {
                return crmService.AuthenticateUser(username, password);

            }
            catch (Exception exception)
            {
                Trace.LogError(exception);
                throw;
            }
        }
    }
}
