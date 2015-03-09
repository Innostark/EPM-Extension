using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using EPM.Extension.Interfaces;
using EPM.Extension.Model;
using EPM.Extension.Model.RequestModels;
using EPM.Extension.Web.Models;

namespace EPM.Extension.Web.Controllers
{
    public class MeteringCodeThresholdController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly IMeteringPointService _meteringCodeService;
        public MeteringCodeThresholdController(ICustomerService customerService, IMeteringPointService meteringCodeService)
        {
            this._customerService = customerService;
            this._meteringCodeService = meteringCodeService;
        }

        // GET: Customer
        [HttpPost]
        public ActionResult Index(MeteringPointSearchRequest request)
        {
            var result = _meteringCodeService.GetMeteringPointsByCustomerId(request);
            MeteringPointViewModel viewModel = new MeteringPointViewModel();
            viewModel.data = result.MeteringPoints;
            viewModel.recordsFiltered = result.TotalCount;
            viewModel.recordsTotal = result.TotalCount;
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            MeteringPointThreshold model = new MeteringPointThreshold();
            return PartialView("_Create",model);
        }
    }
}