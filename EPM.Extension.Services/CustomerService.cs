using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Services
{
    using Interfaces;
    using Model;
    public class CustomerService: ICustomerService
    {
        private static List<Customer> customers;
         static CustomerService()
        {
            customers = new List<Customer>();
            customers.Add(new Customer { Id = 1, City = "Lahore", Name = "Test 1", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { Id = 2, City = "Islamabad", Name = "Test 2", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { Id = 3, City = "Karachi", Name = "Test 3", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { Id = 4, City = "Faisalabad", Name = "Test 4", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { Id = 5, City = "Peshawar", Name = "Test 5", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { Id = 6, City = "Quetta", Name = "Test 6", Address = "Johar Town", Number = "123", ZipCode = "54000" });

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
    }
}
