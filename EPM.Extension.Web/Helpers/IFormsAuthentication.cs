using System;

namespace  EPM.Extension.Web.Helpers
{
    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie, String userDataString);
        void SignOut();
    }
}
