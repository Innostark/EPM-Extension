using System;
using System.Collections.Generic;
using System.Globalization;
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
        public ActionResult Edit(string id)
        {
            Guid guidOutput;
            bool isGuid = Guid.TryParse(id, out guidOutput);
            MeteringPoint meteringCode = isGuid ? this._meteringCodeService.GetMeteringPointsById(guidOutput) : this._meteringCodeService.GetMeteringPointsByCode(id);
            return View(meteringCode);
        }

        [HttpGet]
        public ActionResult StandardInfo(string id)
        {
            Guid guidOutput;
            bool isGuid = Guid.TryParse(id, out guidOutput);
            MeteringPoint meteringCode = isGuid ? this._meteringCodeService.GetMeteringPointsById(guidOutput) : this._meteringCodeService.GetMeteringPointsByCode(id);
            return View(meteringCode);
        }


        [HttpPost]
        public bool StandardInfo(MeteringPoint meteringPoint)
        {
            
            return true;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Detail(string id)
        {
            const string defaultReport = "0";
            string selectedValue = defaultReport;
            string aktivId = ((int)MetadataGrenzwert.OpSetReport.Aktiv).ToString(CultureInfo.InvariantCulture);
            string inAktivId = ((int)MetadataGrenzwert.OpSetReport.NeinAktiv).ToString(CultureInfo.InvariantCulture);
            MeteringPoint mp = _meteringCodeService.GetMeteringPointsByCode(id);
            selectedValue = _meteringCodeService.GetReportSelectedValue(mp, defaultReport, aktivId, inAktivId);

            var list = new SelectList(new[]
                                      {
                                          new{ID=defaultReport, Name="- Please Select -"},
                                          new{ID= aktivId, Name= Resources.MeteringCodeThreshold.Active},
                                          new{ID= inAktivId, Name= Resources.MeteringCodeThreshold.InActive},
                                      }, "ID", "Name", selectedValue);
            ViewBag.List = list;
            return View(mp);
        }        
    }
}