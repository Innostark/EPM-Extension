﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPM.Extension.Interfaces;
using EPM.Extension.Model;
using EPM.Extension.Model.RequestModels;
using EPM.Extension.Web.Models;

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
            CustomerViewModel viewModel = new CustomerViewModel();
            viewModel.SearchRequest=new CustomerSearchRequest();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Index(CustomerSearchRequest request)
        {
            CustomerViewModel viewModel = new CustomerViewModel();

            var cList = customerService.FindCustomers(request);
            viewModel.data = cList.Customers;
            viewModel.recordsTotal = cList.TotalCount;
            viewModel.recordsFiltered = cList.TotalCount;
            return Json(viewModel, JsonRequestBehavior.AllowGet);
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