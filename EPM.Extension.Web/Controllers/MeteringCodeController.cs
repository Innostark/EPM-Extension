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
    public class MeteringCodeController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly IMeteringPointService _meteringCodeService;
        public MeteringCodeController(IMeteringPointService service, ICustomerService customerService, IMeteringPointService meteringCodeService)
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
        public ActionResult Edit(Guid id)
        {
            MeteringPoint meteringCode = this._meteringCodeService.GetMeteringPointsById(id);
            return View(meteringCode);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Detail(string id)
        {
            var meteringPoint = new MeteringPoint
            {
                MeteringCodeThresholds = new List<MeteringPointThreshold>() { new MeteringPointThreshold() {Id = Guid.NewGuid(), Type = MeteringPointThresholdType.System}, new MeteringPointThreshold() { Id = Guid.NewGuid(), Type = MeteringPointThresholdType.User} }
            };

            return View(meteringPoint);
        }
    }
}