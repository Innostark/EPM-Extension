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
        public IEnumerable<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            customers.Add(new Customer { City = "Lahore", Name = "Test 1", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { City = "Islamabad", Name = "Test 2", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { City = "Karachi", Name = "Test 3", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { City = "Faisalabad", Name = "Test 4", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { City = "Peshawar", Name = "Test 5", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            customers.Add(new Customer { City = "Quetta", Name = "Test 6", Address = "Johar Town", Number = "123", ZipCode = "54000" });
            return customers;
        }
    }
}
