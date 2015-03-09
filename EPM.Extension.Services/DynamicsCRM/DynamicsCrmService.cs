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
                    var accounts = serviceContext.CreateQuery(EntityNames.Account).Where(e => ((string)e[MetadataAccount.NAME]) != String.Empty);
                    foreach (var account in accounts)
                    {
                        CrmAccount crmAccount = new CrmAccount();
                        if (account.Contains(MetadataAccount.NAME))
                        {
                            crmAccount.Kunde = account.GetAttributeValue<string>(MetadataAccount.NAME);
                        }
                        if (account.Contains(MetadataAccount.KUNDENNUMMER))
                        {
                            crmAccount.Kundennummer = account.GetAttributeValue<string>(MetadataAccount.KUNDENNUMMER);
                        }
                        if (account.Contains(MetadataAccount.ORT))
                        {
                            crmAccount.Ort = account.GetAttributeValue<string>(MetadataAccount.ORT);
                        }
                        if (account.Contains(MetadataAccount.PLZ))
                        {
                            crmAccount.Plz = account.GetAttributeValue<string>(MetadataAccount.PLZ);
                        }
                        if (account.Contains(MetadataAccount.STRASSE))
                        {
                            crmAccount.Strasse = account.GetAttributeValue<string>(MetadataAccount.STRASSE);
                        }

                        crmAccounts.Add(crmAccount);
                    }
                }
            }

            return crmAccounts;
        }

        public List<MeteringPoint> GetMeteringCodesForAccount(Guid crmAccountId)
        {
            List<MeteringPoint> meteringCodes = new List<MeteringPoint>();

            using (var serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (var serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    //var zahplunkts = serviceContext.CreateQuery(EntityNames.ZAHLPUNKT).Where(za => za.Contains(MetadataKunden.ACCOUNT)
                    //                                                                       && za.GetAttributeValue<EntityReference>(MetadataKunden.ACCOUNT).Id == crmAccountId);
                    //foreach (var zahplunkt in zahplunkts)
                    //{
                    //    MeteringCode meteringCode = new MeteringCode();
                    //    if (zahplunkt.Contains(MetadataKunden.ORT))
                    //    {
                    //        meteringCode.Ort = zahplunkt.GetAttributeValue<string>(MetadataKunden.ORT);
                    //    }

                    //    if (zahplunkt.Contains(MetadataKunden.PLZ))
                    //    {
                    //        meteringCode.PLZ = zahplunkt.GetAttributeValue<string>(MetadataKunden.PLZ);
                    //    }

                    //    if (zahplunkt.Contains(MetadataKunden.STRASSE))
                    //    {
                    //        meteringCode.Strasse = zahplunkt.GetAttributeValue<string>(MetadataKunden.STRASSE);
                    //    }

                    //    meteringCode.CustomerId = crmAccountId;

                    //    meteringCodes.Add(meteringCode);
                    //}
                }
            }

            return meteringCodes;
        }

        public List<MeteringPoint> GetMeteringPoints()
        {
            List<MeteringPoint> meteringPoints = new List<MeteringPoint>();

            using (var serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (var serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    IQueryable<Entity> zahplunkts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt);
                    foreach (Entity zahplunkt in zahplunkts)
                    {
                        MeteringPoint meteringPoint = new MeteringPoint();

                        if (zahplunkt.Contains(MetadataDZählpunkt.NAME))
                        {
                            meteringPoint.Name = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.NAME);
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.ACCOUNT))
                        {
                            EntityReference linkedAccount = zahplunkt.GetAttributeValue<EntityReference>(MetadataDZählpunkt.ACCOUNT);
                            meteringPoint.CrmAccountId = linkedAccount.Id;
                            meteringPoint.CrmAccountName = linkedAccount.Name;
                        }    
                        if (zahplunkt.Contains(MetadataDZählpunkt.ZAHLPUNKTBEZEICHNER))
                        {
                            meteringPoint.Zählpunktbezeichner = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.ZAHLPUNKTBEZEICHNER);
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.KURZEEZEICHNUNG))
                        {
                            meteringPoint.Kurzbezeichnung = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.KURZEEZEICHNUNG);
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.ANLAGENTYP))
                        {
                            meteringPoint.Anlagentyp = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.ANLAGENTYP);
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.PLZ))
                        {
                            meteringPoint.PLZ = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.PLZ);
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.ORT))
                        {
                            meteringPoint.Ort = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.ORT);
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.DATENVERSANDAKTIV))
                        {
                            meteringPoint.DatenversandAktiv = zahplunkt.GetAttributeValue<bool>(MetadataDZählpunkt.DATENVERSANDAKTIV)? "Yes": "No";
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.ZAHLVERFAHREN))
                        {
                            string value = String.Empty;
                            int code = -1;
                            switch (zahplunkt.GetAttributeValue<OptionSetValue>(MetadataDZählpunkt.ZAHLVERFAHREN).Value)
                            {
                                case (int)MetadataDZählpunkt.OpSetZählverfahren.RLM:
                                    code = (int)MetadataDZählpunkt.OpSetZählverfahren.RLM;
                                    value = MetadataDZählpunkt.OpSetZählverfahren.RLM.ToString();
                                    break;
                                case (int)MetadataDZählpunkt.OpSetZählverfahren.SLP:
                                    code = (int)MetadataDZählpunkt.OpSetZählverfahren.SLP;
                                    value = MetadataDZählpunkt.OpSetZählverfahren.SLP.ToString();
                                    break;
                            }
                            meteringPoint.ZählverfahrenCode = code;
                            meteringPoint.ZählverfahrenValue = value;
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.UMESSUNG))
                        {
                            int code; string value;
                            SetSetSpannungsebene(zahplunkt.GetAttributeValue<OptionSetValue>(MetadataDZählpunkt.UMESSUNG).Value, 
                                                out code, out value);

                            meteringPoint.UMessungCode = code;
                            meteringPoint.UMessungValue = value;
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.UENTNAHME))
                        {
                            int code; string value;
                            SetSetSpannungsebene(zahplunkt.GetAttributeValue<OptionSetValue>(MetadataDZählpunkt.UENTNAHME).Value,
                                                out code, out value);

                            meteringPoint.UEntnahmeCode = code;
                            meteringPoint.UEntnahmeValue = value;
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.KUNDENRUCKMELDUNG))
                        {
                            meteringPoint.Kundenrückmeldung = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.KUNDENRUCKMELDUNG);
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.VNB))
                        {
                            EntityReference linkedVnb = zahplunkt.GetAttributeValue<EntityReference>(MetadataDZählpunkt.VNB);
                            meteringPoint.VNBId = linkedVnb.Id;
                            meteringPoint.VNBName = linkedVnb.Name;
                        }
                        if (zahplunkt.Contains(MetadataDZählpunkt.CODE))
                        {
                            meteringPoint.Code = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.CODE);
                        }

                        //Get the threshold for this metering point
                        IQueryable<Entity> grenzwerts = serviceContext.CreateQuery(EntityNames.Grenzwert).Where(g => g.GetAttributeValue<EntityReference>(MetadataGrenzwert.GrenzwerteZPID).Id == zahplunkt.Id);
                        List<MeteringPointThreshold> meteringPointThresholds = new List<MeteringPointThreshold>();
                        if(grenzwerts != null)
                        {
                            Entity grenzwert = grenzwerts.ToList().FirstOrDefault();
                            if (grenzwert != null)
                            {
                                MeteringPointThreshold meteringPointThreshlodSystem = new MeteringPointThreshold();
                                if (grenzwert.Contains(MetadataGrenzwert.Grenze))
                                {
                                    meteringPointThreshlodSystem.GrenzwertBezeichner = String.Format("{0:F2}", grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.Grenze));
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GültigAb))
                                {
                                    meteringPointThreshlodSystem.GultingAb = grenzwert.GetAttributeValue<DateTime>(MetadataGrenzwert.GültigAb);
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMaxSystem))
                                {
                                    meteringPointThreshlodSystem.MaximaGlobal = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMaxSystem)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMinSystem))
                                {
                                    meteringPointThreshlodSystem.MinimaGlobal = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMinSystem)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMaxSystem))
                                {
                                    meteringPointThreshlodSystem.MaximaSommer = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMaxSystem)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMinSystem))
                                {
                                    meteringPointThreshlodSystem.MinimaSommer = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMinSystem)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMaxSystem))
                                {
                                    meteringPointThreshlodSystem.MaximaWinter = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMaxSystem)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMinSystem))
                                {
                                    meteringPointThreshlodSystem.MinimaWinter = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMinSystem)).ToString();
                                }
                                meteringPointThresholds.Add(meteringPointThreshlodSystem);

                                MeteringPointThreshold meteringPointThreshlodUser = new MeteringPointThreshold();
                                if (grenzwert.Contains(MetadataGrenzwert.Grenze))
                                {
                                    meteringPointThreshlodUser.GrenzwertBezeichner = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.Grenze)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GültigAb))
                                {
                                    meteringPointThreshlodUser.GultingAb = grenzwert.GetAttributeValue<DateTime>(MetadataGrenzwert.GültigAb);
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMaxUser))
                                {
                                    meteringPointThreshlodUser.MaximaGlobal = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMaxUser)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMinUser))
                                {
                                    meteringPointThreshlodUser.MinimaGlobal = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMinUser)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMaxUser))
                                {
                                    meteringPointThreshlodUser.MaximaSommer = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMaxUser)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMinUser))
                                {
                                    meteringPointThreshlodUser.MinimaSommer = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMinUser)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMaxUser))
                                {
                                    meteringPointThreshlodUser.MaximaWinter = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMaxUser)).ToString();
                                }
                                if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMinUser))
                                {
                                    meteringPointThreshlodUser.MinimaWinter = (grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMinUser)).ToString();
                                }
                                meteringPointThresholds.Add(meteringPointThreshlodUser);

                                meteringPoint.MeteringCodeThresholds = meteringPointThresholds;
                            }
                        }
                        meteringPoints.Add(meteringPoint);
                    }
                }
            }

            return meteringPoints;
        }

        private static void SetSetSpannungsebene(int caseValue, out int code, out string value)
        {
            value = String.Empty;
            code = -1;
            switch (caseValue)
            {
                case (int)MetadataOpSetSpannungsebene.MS:
                    code = (int)MetadataOpSetSpannungsebene.MS;
                    value = MetadataOpSetSpannungsebene.MS.ToString();
                    break;
                case (int)MetadataOpSetSpannungsebene.HS:
                    code = (int)MetadataOpSetSpannungsebene.HS;
                    value = MetadataOpSetSpannungsebene.HS.ToString();
                    break;
                case (int)MetadataOpSetSpannungsebene.HöS:
                    code = (int)MetadataOpSetSpannungsebene.HöS;
                    value = MetadataOpSetSpannungsebene.HöS.ToString();
                    break;
                case (int)MetadataOpSetSpannungsebene.MSNS:
                    code = (int)MetadataOpSetSpannungsebene.MSNS;
                    value = "MS/NS";
                    break;
                case (int)MetadataOpSetSpannungsebene.HSMS:
                    code = (int)MetadataOpSetSpannungsebene.HSMS;
                    value = "HS/MS";
                    break;
                case (int)MetadataOpSetSpannungsebene.HöSHS:
                    code = (int)MetadataOpSetSpannungsebene.HöSHS;
                    value = "HöS/HS";
                    break;
            }
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
