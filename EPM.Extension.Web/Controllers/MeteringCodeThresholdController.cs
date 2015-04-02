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
   // [Authorize]
    public class MeteringCodeThresholdController : BaseController
    {
        private readonly IMeteringPointThresholdService _meteringPointThresholdService;
        public MeteringCodeThresholdController(IMeteringPointThresholdService meteringCodeService)
        {
            this._meteringPointThresholdService = meteringCodeService;
        }


        [HttpGet]
        public ActionResult Edit(Guid id, bool? header)
        {
            if (Request.UrlReferrer != null)
            {
                ViewBag.ShowHeader = Request.UrlReferrer.ToString().Contains("Edit");
            }
           
            
            MeteringPointThreshold model = _meteringPointThresholdService.GetMeteringPointThresholdById(id);
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(MeteringPointThreshold model)
        {
            if (ModelState.IsValid)
            {
                _meteringPointThresholdService.UpdateMeteringThreshold(model);
                return Content("");
            }
            return PartialView("_Edit", model);
        }
    }
}