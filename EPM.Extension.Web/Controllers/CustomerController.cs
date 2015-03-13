using System;
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
    [Authorize]
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
            viewModel.BetriebeSearchRequest = new CustomerSearchRequest();
            viewModel.MeteringCodeSearchRequest=new MeteringPointSearchRequest();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Index(CustomerSearchRequest request)
        {
            CustomerViewModel viewModel = new CustomerViewModel();

            var cList = customerService.FindCustomers(request, LoggedInUserId);
            viewModel.data = cList.Customers;
            viewModel.recordsTotal = cList.TotalCount;
            viewModel.recordsFiltered = cList.TotalCount;
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(Guid id)
        {
            CrmAccount customer = this.customerService.GetCustomerById(id);
            CustomerDetailViewModel model= new CustomerDetailViewModel
            {
                Customer = customer,
                SearchRequest = new MeteringPointSearchRequest(),
                BetriebeSearchRequest = new CustomerSearchRequest()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CrmAccount  customer)
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