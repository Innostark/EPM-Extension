using System.Web.Mvc;
using EPM.Extension.Interfaces;
using EPM.Extension.Web.Controllers;
using EPM.Extension.Web.Helpers;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using EPM.Extension.Services;
namespace EPM.Extension.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<IMeteringPointService, MeteringPointService>();
            container.RegisterType<IFormsAuthentication, FormAuthenticationService>();
            container.RegisterType<IMeteringPointThresholdService, MeteringPointThresholdService>();
            // e.g. container.RegisterType<ITestService, TestService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}