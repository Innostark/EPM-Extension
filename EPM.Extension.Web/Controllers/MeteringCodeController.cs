using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPM.Extension.Interfaces;
using EPM.Extension.Model;

namespace EPM.Extension.Web.Controllers
{
    public class MeteringCodeController : BaseController
    {
        private readonly ICustomerService _customerService;
        private readonly IMeteringCodeService _meteringCodeService;
        public MeteringCodeController(IMeteringCodeService service, ICustomerService customerService, IMeteringCodeService meteringCodeService)
        {
            this._customerService = customerService;
            this._meteringCodeService = meteringCodeService;
        }

        // GET: Customer
        public ActionResult Index()
        {
            
            return View(_customerService.GetAllCustomers());
        }
    }
}