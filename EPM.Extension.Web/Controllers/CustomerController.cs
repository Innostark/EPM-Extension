using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPM.Extension.Interfaces;
using EPM.Extension.Model;

namespace EPM.Extension.Web.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService service, ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        // GET: Customer
        public ActionResult Index()
        {
            
            return View(customerService.GetAllCustomers());
        }


        public ActionResult Edit(int id)
        {
            Customer customer = this.customerService.GetCustomerById(id);
            return View(customer);
        }

        [HttpPost]
        public ActionResult Edit(Customer  customer)
        {
            if (ModelState.IsValid)
            {
                customerService.UpdateCustomer(customer);
                return RedirectToAction("Index");
            }
            
            return View(customer);
        }
    }
}