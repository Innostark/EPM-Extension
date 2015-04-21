﻿using System;
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
        public ActionResult Edit(Guid id)
        {
            if (Request.UrlReferrer != null)
            {
                ViewBag.ShowHeader = Request.UrlReferrer.ToString().Contains("Edit");
            }
           
            
            MeteringPointThreshold model = _meteringPointThresholdService.GetMeteringPointThresholdById(id);
            Response.CacheControl = "no-cache";
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(MeteringPointThreshold model)
        {
            if (!string.IsNullOrEmpty(model.GultingAbCopy))
            {
                var str = model.GultingAbCopy.Split('-');
                model.GultingAb = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]), Convert.ToInt32(str[0]));
            }
            if (ModelState.IsValid)
            {
                _meteringPointThresholdService.UpdateMeteringThreshold(model);
                return Content("");
            }
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public ActionResult SaveReport(Guid id, int option, string Empfaenger1, string Empfaenger2, string Empfaenger3)
        {
            if (ModelState.IsValid)
            {
                _meteringPointThresholdService.SaveMeteringPointThresholdReport(id, option,Empfaenger1,Empfaenger2,Empfaenger3);
            }
            return Content("");
        }
    }
}