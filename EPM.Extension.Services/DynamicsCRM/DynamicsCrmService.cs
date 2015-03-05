using EPM.Extension.Model;
using EPM.Extension.Services.DynamicsCRM.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;

namespace EPM.Extension.Services.DynamicsCRM
{
    public class DynamicsCrmService
    {
        public List<CrmAccount> GetAccounts()
        {
            List<CrmAccount> crmAccounts = new List<CrmAccount>();
            using (var serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (var serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    var accounts = serviceContext.CreateQuery(EntityNames.ACCOUNT).Where(e => ((string)e[MetadataAccount.NAME]) != String.Empty);
                    foreach (var account in accounts)
                    {
                        CrmAccount crmAccount = new CrmAccount();
                        if (account.Contains(MetadataAccount.NAME))
                        {
                            crmAccount.Kunde = account.GetAttributeValue<string>(MetadataAccount.NAME);
                        }
                        
                        if(account.Contains(MetadataAccount.KUNDERNUMMER))
                        {
                            crmAccount.Kundennummer = account.GetAttributeValue<string>(MetadataAccount.KUNDERNUMMER);
                        }

                        crmAccounts.Add(crmAccount);
                    }
                }
            }

            return crmAccounts;
        }

        public List<MeteringCode> GetMeteringCodesForAccount(Guid crmAccountId)
        {
            List<MeteringCode> meteringCodes = new List<MeteringCode>();

            using (var serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (var serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    var zahplunkts = serviceContext.CreateQuery(EntityNames.ZAHLPUNKT).Where(za => za.Contains(MetadataKunderNumber.ACCOUNT)
                                                                                           && za.GetAttributeValue<EntityReference>(MetadataKunderNumber.ACCOUNT).Id == crmAccountId);
                    foreach (var zahplunkt in zahplunkts)
                    {
                        MeteringCode meteringCode = new MeteringCode();
                        if (zahplunkt.Contains(MetadataKunderNumber.ORT))
                        {
                            meteringCode.Ort = zahplunkt.GetAttributeValue<string>(MetadataKunderNumber.ORT);
                        }

                        if (zahplunkt.Contains(MetadataKunderNumber.PLZ))
                        {
                            meteringCode.PLZ = zahplunkt.GetAttributeValue<string>(MetadataKunderNumber.PLZ);
                        }

                        if (zahplunkt.Contains(MetadataKunderNumber.STRASSE))
                        {
                            meteringCode.Strasse = zahplunkt.GetAttributeValue<string>(MetadataKunderNumber.STRASSE);
                        }

                        meteringCode.CustomerId = crmAccountId;

                        meteringCodes.Add(meteringCode);
                    }
                }
            }

            return meteringCodes;
        }

        #region Dynamics CRM Connection
        private static OrganizationServiceProxy GetProxyService()
        {
            IServiceManagement<IDiscoveryService> serviceManagement = DynamicsCrmService.GetDiscoveryService("http://10.10.30.6:5555");
            IServiceManagement<IOrganizationService> orgServiceManagement = DynamicsCrmService.GetOrganisationService("http://10.10.30.6:5555", "crm-epm");
            AuthenticationCredentials authenticationCredentials = GetCredentials(serviceManagement.AuthenticationType, "CRM-001", "Energy4321!", "ENOMOS");

            return GetProxy<IOrganizationService, OrganizationServiceProxy>(orgServiceManagement, authenticationCredentials);
        }

        private static IServiceManagement<IOrganizationService> GetOrganisationService(string serverPath, string organisationName)
        {
            return ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(string.Format("{0}/{1}/XRMServices/2011/Organization.svc", serverPath, organisationName)));
        }

        private static IServiceManagement<IDiscoveryService> GetDiscoveryService(string serverPath)
        {
            return ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(new Uri(string.Format("{0}/XRMServices/2011/Discovery.svc", serverPath)));
        }

        private static AuthenticationCredentials GetCredentials(AuthenticationProviderType endpointType, string username, string password, string domain)
        {

            AuthenticationCredentials authCredentials = new AuthenticationCredentials();
            switch (endpointType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    authCredentials.ClientCredentials.Windows.ClientCredential =
                        new System.Net.NetworkCredential(username, password, domain);
                    break;
                //case AuthenticationProviderType.LiveId:
                //    authCredentials.ClientCredentials.UserName.UserName = username;
                //    authCredentials.ClientCredentials.UserName.Password = password;
                //    authCredentials.SupportingCredentials = new AuthenticationCredentials();
                //    authCredentials.SupportingCredentials.ClientCredentials =
                //        Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
                //    break;
                default: // For Federated and OnlineFederated environments.                    
                    authCredentials.ClientCredentials.UserName.UserName = username;
                    authCredentials.ClientCredentials.UserName.Password = password;
                    // For OnlineFederated single-sign on, you could just use current UserPrincipalName instead of passing user name and password.
                    // authCredentials.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;  //Windows/Kerberos
                    break;
            }

            return authCredentials;
        }
        private static TProxy GetProxy<TService, TProxy>(IServiceManagement<TService> serviceManagement, AuthenticationCredentials authCredentials)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            var classType = typeof(TProxy);

            if (serviceManagement.AuthenticationType != AuthenticationProviderType.ActiveDirectory)
            {
                AuthenticationCredentials tokenCredentials = serviceManagement.Authenticate(authCredentials);
                return (TProxy)classType.GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) }).Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
            }

            return (TProxy)classType.GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) }).Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }
        #endregion Dynamics CRM Connection

        #region CRMUTILCODE-NOT REQUIRED FOR NOW
        //public Configuration configuration = new Configuration();

        //public class Configuration
        //{
        //    public string ServerAddress;
        //    public string OrganizationName;
        //    public Uri DiscoveryUri;
        //    public Uri OrganisationUri;
        //    public string OrganisationName;
        //    public AuthenticationCredentials Credentials;

        //}

        //public virtual Configuration GetServerConfiguration(string serverAddress, string organisationName,
        //                                                    AuthenticationProviderType endpointType, string username, string password, string domain)
        //{
        //    this.configuration.ServerAddress = serverAddress;
        //    this.configuration.OrganisationName = organisationName;
        //    this.configuration.DiscoveryUri = new Uri(String.Format("{0}/XRMServices/2011/Discovery.svc", configuration.ServerAddress));
        //    this.configuration.Credentials = DynamicsCrmService.GetCredentials(endpointType, username, password, domain);
        //    string organisationUrl = String.Format("{0}/{1}/XRMServices/2011/Organization.svc", serverAddress, organisationName);
        //    Uri uri = new Uri(organisationUrl);
        //    this.configuration.OrganisationUri = uri;

        //    return configuration;
        //}

        //public virtual IOrganizationService GetService(Configuration config)
        //{
        //    var serviceManagement = ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(config.DiscoveryUri);

        //    return (IOrganizationService)ServiceConfigurationFactory.CreateManagement<IOrganizationService>(config.OrganisationUri);
        //}

        //public virtual OrganizationServiceProxy GetOrganisationServiceProxy(IServiceManagement<IOrganizationService> service, AuthenticationCredentials credentials)
        //{
        //    return DynamicsCrmService.GetProxy<IOrganizationService, OrganizationServiceProxy>(service, credentials);
        //}

        //private static AuthenticationCredentials GetCredentials(AuthenticationProviderType endpointType, string username, string password, string domain)
        //{
        //    AuthenticationCredentials authCredentials = new AuthenticationCredentials();
        //    switch (endpointType)
        //    {
        //        case AuthenticationProviderType.ActiveDirectory:
        //            authCredentials.ClientCredentials.Windows.ClientCredential =
        //                new System.Net.NetworkCredential(username, password, domain);
        //            break;
        //        //case AuthenticationProviderType.LiveId:
        //        //    authCredentials.ClientCredentials.UserName.UserName = username;
        //        //    authCredentials.ClientCredentials.UserName.Password = password;
        //        //    authCredentials.SupportingCredentials = new AuthenticationCredentials();
        //        //    authCredentials.SupportingCredentials.ClientCredentials =
        //        //        Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
        //        //    break;
        //        default: // For Federated and OnlineFederated environments.                    
        //            authCredentials.ClientCredentials.UserName.UserName = username;
        //            authCredentials.ClientCredentials.UserName.Password = password;
        //            // For OnlineFederated single-sign on, you could just use current UserPrincipalName instead of passing user name and password.
        //            // authCredentials.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;  //Windows/Kerberos
        //            break;
        //    }

        //    return authCredentials;
        //}

        //private static TProxy GetProxy<TService, TProxy>(IServiceManagement<TService> serviceManagement, AuthenticationCredentials authCredentials)
        //    where TService : class
        //    where TProxy : ServiceProxy<TService>
        //{
        //    var classType = typeof(TProxy);

        //    if (serviceManagement.AuthenticationType != AuthenticationProviderType.ActiveDirectory)
        //    {
        //        AuthenticationCredentials tokenCredentials = serviceManagement.Authenticate(authCredentials);
        //        return (TProxy)classType.GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) }).Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
        //    }

        //    return (TProxy)classType.GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) }).Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        //} 
        #endregion
    }
}
