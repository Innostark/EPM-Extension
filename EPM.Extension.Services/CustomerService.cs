using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Services
{
    using EPM.Extension.Model.Common;
    using Interfaces;
    using Model;
    using System.Linq.Expressions;
    public class CustomerService: ICustomerService
    {
        private static List<Customer> customers;
        private readonly Dictionary<CustomerColumnBy, Func<Customer, object>> userActivityClause =
                  new Dictionary<CustomerColumnBy, Func<Customer, object>>
                    {
                        {CustomerColumnBy.Name, c => c.Name},
                        {CustomerColumnBy.Number, c => c.Number},
                        {CustomerColumnBy.Address, c => c.Address},
                        {CustomerColumnBy.ZipCode, c => c.ZipCode},
                        {CustomerColumnBy.City, c => c.City}
                    };
         static CustomerService()
        {
            customers = new List<Customer>();
            customers.Add(new Customer { Id = 1, City = "Lahore", Name = "Test 1", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { Id = 2, City = "Islamabad", Name = "Test 2", Address = "Johar Town", Number = "456", ZipCode = "64000" });
            customers.Add(new Customer { Id = 3, City = "Karachi", Name = "Test 3", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { Id = 4, City = "Faisalabad", Name = "Test 4", Address = "Johar Town", Number = "789", ZipCode = "64000" });
            customers.Add(new Customer { Id = 5, City = "Peshawar", Name = "Test 5", Address = "Johar Town", Number = "012", ZipCode = "64000" });
            customers.Add(new Customer { Id = 6, City = "Quetta", Name = "Test 6", Address = "Johar Town", Number = "123", ZipCode = "74000" });
            customers.Add(new Customer { Id = 7, City = "Lahore", Name = "Test 7", Address = "Johar Town", Number = "234", ZipCode = "74000" });
            customers.Add(new Customer { Id = 8, City = "Islamabad", Name = "Test 8", Address = "Johar Town", Number = "345", ZipCode = "54000" });
            customers.Add(new Customer { Id = 9, City = "Karachi", Name = "Test 9", Address = "Johar Town", Number = "456", ZipCode = "64000" });
            customers.Add(new Customer { Id = 10, City = "Faisalabad", Name = "Test 10", Address = "Johar Town", Number = "567", ZipCode = "44000" });
            customers.Add(new Customer { Id = 11, City = "Peshawar", Name = "Test 11", Address = "Johar Town", Number = "678", ZipCode = "64000" });
            customers.Add(new Customer { Id = 12, City = "Quetta", Name = "Test 12", Address = "Johar Town", Number = "789", ZipCode = "54000" });

        }
        public IEnumerable<Customer> GetAllCustomers()
        {
            return customers;
        }

        public void UpdateCustomer(Customer customer)
        {
            Customer c = customers.FirstOrDefault(x => x.Id == customer.Id);
            var index = customers.IndexOf(c);
            customers.Remove(c);
            customers.Insert(index, customer);
        }


        public Customer GetCustomerById(int id)
        {
            return customers.FirstOrDefault(x => x.Id == id);
        }


        public CustomerResponse FindCustomers(Model.RequestModels.CustomerSearchRequest searchRequest)
        {
            int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
            int toRow = searchRequest.PageSize;

            Func<Customer, bool> expression =
                s => (string.IsNullOrEmpty(searchRequest.Param) || s.Name.Contains(searchRequest.Param) || s.Number.Contains(searchRequest.Param) || s.Address.Contains(searchRequest.Param) || s.City.Contains(searchRequest.Param));
            
                IEnumerable<Customer> oList =
                searchRequest.IsAsc ?
                customers.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
                customers.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();
                return new  CustomerResponse{ Customers = oList, TotalCount = customers.Where(expression).ToList().Count };
        }
    }
}
