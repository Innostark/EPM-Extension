using System.Web.Mvc;
using EPM.Extension.Interfaces;
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
            container.RegisterType<IMeteringCodeService, MeteringCodeService>();
            // e.g. container.RegisterType<ITestService, TestService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}