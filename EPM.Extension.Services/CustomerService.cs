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
        DynamicsCrmService crmService = new DynamicsCrmService();
        private readonly Dictionary<CustomerColumnBy, Func<CrmAccount, object>> userActivityClause =
                  new Dictionary<CustomerColumnBy, Func<CrmAccount, object>>
                    {
                        {CustomerColumnBy.Name, c => c.Kunde},
                        {CustomerColumnBy.Number, c => c.Kundennummer},
                        {CustomerColumnBy.Address, c => c.Strasse},
                        {CustomerColumnBy.ZipCode, c => c.Plz},
                        {CustomerColumnBy.City, c => c.Ort}
                    };
         static CustomerService()
        {
            DynamicsCrmService crmService = new DynamicsCrmService();
            customers = crmService.GetAccounts();
          
        }
        public IEnumerable<CrmAccount> GetAllCustomers()
        {
            return customers;
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
            return customers.FirstOrDefault(x => x.Id == id);
        }


        public CustomerResponse FindCustomers(Model.RequestModels.CustomerSearchRequest searchRequest)
        {
            int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
            int toRow = searchRequest.PageSize;
            bool searchSpecified = !string.IsNullOrEmpty(searchRequest.Param);
            Func<CrmAccount, bool>  expression =
                s => (
                    searchSpecified &&
                    (!string.IsNullOrEmpty(s.Kunde) && s.Kunde.ToLower().Contains(searchRequest.Param.ToLower())) 
                    || !searchSpecified);
            
                IEnumerable<CrmAccount> oList =
                searchRequest.IsAsc ?
                customers.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
                customers.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();
                return new  CustomerResponse{ Customers = oList, TotalCount = customers.Where(expression).ToList().Count };
        }

        public CustomerResponse FindBetrieber(Model.RequestModels.CustomerSearchRequest searchRequest)
        {
            int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
            int toRow = searchRequest.PageSize;
            bool searchSpecified = !string.IsNullOrEmpty(searchRequest.Param);
            List<CrmAccount> betriebers = crmService.GetBeitreibersByAccountId(searchRequest.CustomerId);
            Func<CrmAccount, bool> expression =
                s => (
                    searchSpecified && 
                    (!string.IsNullOrEmpty(s.Kunde) && s.Kunde.ToLower().Contains(searchRequest.Param.ToLower()))
                    || !searchSpecified);

            IEnumerable<CrmAccount> oList =
            searchRequest.IsAsc ?
            betriebers.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
            betriebers.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();
            return new CustomerResponse { Customers = oList, TotalCount = betriebers.Where(expression).ToList().Count };
        }


        public CrmAccount GetAccount(string username, string password)
        {
            return new CrmAccount
            {
                Id = Guid.NewGuid(),
                Kunde = "test",
                Kundennummer = "testnumber"
            };
        }
    }
}
