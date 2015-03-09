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
        private readonly IMeteringCodeService _meteringCodeService;
        public MeteringCodeThresholdController(IMeteringCodeService service, ICustomerService customerService, IMeteringCodeService meteringCodeService)
        {
            this._customerService = customerService;
            this._meteringCodeService = meteringCodeService;
        }

        // GET: Customer
        [HttpPost]
        public ActionResult Index(MeteringCodeSearchRequest request)
        {
            var result = _meteringCodeService.GetMeteringCodesByCustomerId(request);
            MeteringCodeViewModel viewModel = new MeteringCodeViewModel();
            viewModel.data = result.MeteringCodes;
            viewModel.recordsFiltered = result.TotalCount;
            viewModel.recordsTotal = result.TotalCount;
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            MeteringCodeThreshold model = new MeteringCodeThreshold();
            return PartialView("_Create",model);
        }
    }
}