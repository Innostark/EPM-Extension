using System;
using System.Web;
using System.Web.Security;

namespace  EPM.Extension.Web.Helpers
{
   public class FormAuthenticationService : IFormsAuthentication
    {

       public void SignIn(string userName, bool createPersistentCookie, String userDataString)
        {
            
            var authTicket = new FormsAuthenticationTicket(
                1,
                userName,  //user id
                DateTime.Now,
                //DateTime.Now.AddMinutes(30),  // expiry
                DateTime.Now.AddDays(15),  // expiry
                createPersistentCookie,
                userDataString,
                "/");

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
            
            if (authTicket.IsPersistent)
            {
                cookie.Expires = authTicket.Expiration;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public void SignOut()
        {
            HttpContext.Current.Session.RemoveAll();
            FormsAuthentication.SignOut();
            
        }
    }
}
