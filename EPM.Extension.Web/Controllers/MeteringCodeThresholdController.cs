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