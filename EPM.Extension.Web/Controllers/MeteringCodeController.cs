using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPM.Extension.Interfaces;
using EPM.Extension.Model;
using EPM.Extension.Model.RequestModels;
using EPM.Extension.Services.DynamicsCRM.Metadata;
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
            MeteringPoint pt = _meteringCodeService.GetMeteringPointsByCode(id);
            var list = new SelectList(new[]
                                      {
                                          new{ID="0",Name="- Please Select -"},
                                          new{ID=MetadataGrenzwert.OpSetReport.Aktiv.ToString(),Name= Resources.MeteringCodeThreshold.Active},
                                          new{ID=MetadataGrenzwert.OpSetReport.NeinAktiv.ToString(),Name= Resources.MeteringCodeThreshold.InActive},
                                      }, "ID", "Name", "0");
            ViewBag.List = list;
            return View(pt);
        }
    }
}