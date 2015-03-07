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
            
             //customers = new List<CrmAccount>();
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Lahore", Kunde = "Test 1", Strasse = "Johar Town", Kundennummer = "123", Plz = "54000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Islamabad", Kunde = "Test 2", Strasse = "Johar Town", Kundennummer = "456", Plz = "64000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Karachi", Kunde = "Test 3", Strasse = "Johar Town", Kundennummer = "123", Plz = "54000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Faisalabad", Kunde = "Test 4", Strasse = "Johar Town", Kundennummer = "789", Plz = "64000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Peshawar", Kunde = "Test 5", Strasse = "Johar Town", Kundennummer = "012", Plz = "64000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Quetta", Kunde = "Test 6", Strasse = "Johar Town", Kundennummer = "123", Plz = "74000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Lahore", Kunde = "Test 7", Strasse = "Johar Town", Kundennummer = "234", Plz = "74000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Islamabad", Kunde = "Test 8", Strasse = "Johar Town", Kundennummer = "345", Plz = "54000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Karachi", Kunde = "Test 9", Strasse = "Johar Town", Kundennummer = "456", Plz = "64000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Faisalabad", Kunde = "Test 10", Strasse = "Johar Town", Kundennummer = "567", Plz = "44000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Peshawar", Kunde = "Test 11", Strasse = "Johar Town", Kundennummer = "678", Plz = "64000" });
            //customers.Add(new CrmAccount { Id = Guid.NewGuid(), Ort = "Quetta", Kunde = "Test 12", Strasse = "Johar Town", Kundennummer = "789", Plz = "54000" });
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

            Func<CrmAccount, bool> expression =
                s => (string.IsNullOrEmpty(searchRequest.Param) || s.Kunde.Contains(searchRequest.Param) || s.Kundennummer.Contains(searchRequest.Param) || s.Strasse.Contains(searchRequest.Param) || s.Ort.Contains(searchRequest.Param));
            
                IEnumerable<CrmAccount> oList =
                searchRequest.IsAsc ?
                customers.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
                customers.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();
                return new  CustomerResponse{ Customers = oList, TotalCount = customers.Where(expression).ToList().Count };
        }
    }
}
