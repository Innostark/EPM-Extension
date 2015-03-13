using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPM.Extension.Web.Controllers
{
    using Interfaces;
    using Model;
    using Model.RequestModels;
    using Models;

    [Authorize]
    public class BetriebeController : BaseController
    {
        private readonly ICustomerService _customerService;
        public BetriebeController(IMeteringPointService service, ICustomerService customerService, IMeteringPointService meteringCodeService)
        {
            this._customerService = customerService;
        }

        // GET: Customer
        [HttpPost]
        public ActionResult Index(CustomerSearchRequest  request)
        {
            CustomerViewModel viewModel = new CustomerViewModel();

            var cList = _customerService.FindBetrieber(request, LoggedInUserId);
            viewModel.data = cList.Customers;
            viewModel.recordsTotal = cList.TotalCount;
            viewModel.recordsFiltered = cList.TotalCount;
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

    }
}