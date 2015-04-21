using EPM.Extension.Model;
using EPM.Extension.Model.Common;
using EPM.Extension.Model.RequestModels;
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
        private readonly Dictionary<CustomerColumnBy, Func<Entity, object>> userActivityClause =
        new Dictionary<CustomerColumnBy, Func<Entity, object>>
        {
            {CustomerColumnBy.Name, c => c.Attributes[MetadataAccount.NAME]},
            {CustomerColumnBy.Number, c => c.Attributes[MetadataAccount.KUNDENNUMMER]},
            {CustomerColumnBy.Address, c => c.Attributes[MetadataAccount.STRASSE]},
            {CustomerColumnBy.ZipCode, c => c.Attributes[MetadataAccount.PLZ]},
            {CustomerColumnBy.City, c => c.Attributes[MetadataAccount.ORT]}
        };

        public Guid AuthenticateUser(string username, string password)
        {
            Guid userId = Guid.Parse("269c6436-01c9-e411-8fd9-00155d65282d");

            //using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            //{
            //    using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
            //    {
            //        IQueryable<Entity> usersQ = serviceContext.CreateQuery(EntityNames.EpmExtensionPortalUser).Where(u => u.GetAttributeValue<string>(MetadataEpmExtensionPortalUser.UserName) == username
            //                                                                                                           && u.GetAttributeValue<string>(MetadataEpmExtensionPortalUser.Password) == password);
            //        try
            //        {
            //            List<Entity> users = usersQ.ToList<Entity>();
            //            if (users.Count == 1)
            //                return users[0].Id;
            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //    }
            //}

            return userId;
        }

        #region "Account"

        public CustomerResponse GetAccountsByUserId(Model.RequestModels.CustomerSearchRequest searchRequest, Guid userId)
        {
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    IQueryable<Entity> accounts = serviceContext.CreateQuery(EntityNames.Account)
                                                    .Where(ac => ac.GetAttributeValue<EntityReference>(MetadataAccount.ACCOUNTEPMEXTENSIONPORTALUSER) != null
                                                              && ac.GetAttributeValue<EntityReference>(MetadataAccount.ACCOUNTEPMEXTENSIONPORTALUSER).Id == userId);

                    int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
                    int toRow = searchRequest.PageSize;
                    bool searchSpecified = !string.IsNullOrEmpty(searchRequest.Param);
                    Func<Entity, bool> expression =
                        s => (
                            searchSpecified &&
                            (s.Contains(MetadataAccount.NAME) &&
                             !string.IsNullOrEmpty(s.GetAttributeValue<string>(MetadataAccount.NAME)) &&
                             s.GetAttributeValue<string>(MetadataAccount.NAME)
                                 .ToLower()
                                 .Contains(searchRequest.Param.ToLower()))
                            || !searchSpecified);

                    IEnumerable<Entity> oList =
                    searchRequest.IsAsc ?
                    accounts.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
                    accounts.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();

                    int recordCount = accounts.Count(expression);

                    return new CustomerResponse { Customers = oList.Select(WrapAccountIntoCrmAccount).ToList(), TotalCount = recordCount, UserTotalCount = recordCount };
                }
            }
        }

        public CustomerResponse GetAccounts(Model.RequestModels.CustomerSearchRequest searchRequest)
        {
            //return new CustomerResponse { Customers = oList, TotalCount = customers.Where(expression).ToList().Count };

            int outRecordCount;
            List<CrmAccount> crmAccounts = new List<CrmAccount>();
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    crmAccounts = this.GetAccounts(serviceContext, searchRequest, out outRecordCount);
                }
            }

            return new CustomerResponse { Customers = crmAccounts, TotalCount = outRecordCount, UserTotalCount = outRecordCount };
        }

        public List<CrmAccount> GetAccounts(OrganizationServiceContext serviceContext, Model.RequestModels.CustomerSearchRequest searchRequest, out int recordCount)
        {
            IQueryable<Entity> accounts = serviceContext.CreateQuery(EntityNames.Account);

            int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
            int toRow = searchRequest.PageSize;
            bool searchSpecified = !string.IsNullOrEmpty(searchRequest.Param);
            Func<Entity, bool> expression =
                s => (
                    searchSpecified &&
                    (s.Contains(MetadataAccount.NAME) &&
                     !string.IsNullOrEmpty(s.GetAttributeValue<string>(MetadataAccount.NAME)) &&
                     s.GetAttributeValue<string>(MetadataAccount.NAME)
                         .ToLower()
                         .Contains(searchRequest.Param.ToLower()))
                    || !searchSpecified);

            IEnumerable<Entity> oList =
            searchRequest.IsAsc ?
            accounts.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
            accounts.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();

            recordCount = accounts.Count(expression);

            return oList.Select(WrapAccountIntoCrmAccount).ToList();
        }

        private CrmAccount WrapAccountIntoCrmAccount(Entity account)
        {
            CrmAccount crmAccount = new CrmAccount();
            if (account.Contains(MetadataAccount.NAME))
            {
                crmAccount.Kunde = account.GetAttributeValue<string>(MetadataAccount.NAME);
            }
            if (account.Contains(MetadataAccount.CRM_ACCOUNT_ID))
            {
                crmAccount.Id = account.GetAttributeValue<Guid>(MetadataAccount.CRM_ACCOUNT_ID);
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

            return crmAccount;
        }

        public CrmAccount GetAccountById(Guid crmAccountId)
        {
            CrmAccount crmAccount = new CrmAccount();
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    crmAccount = GetAccountById(serviceContext, crmAccountId);
                }
            }

            return crmAccount;
        }

        private CrmAccount GetAccountById(OrganizationServiceContext serviceContext, Guid crmAccountId)
        {
            var accounts = serviceContext.CreateQuery(EntityNames.Account).Where(e => ((Guid?)e[MetadataAccount.CRM_ACCOUNT_ID]) == crmAccountId);
            foreach (var account in accounts)
            {
                return this.WrapAccountIntoCrmAccount(account);
            }

            return null;
        }

        #endregion "Account"

        #region "Beitreibers"
        public CustomerResponse GetBeitreibersByUserId(Model.RequestModels.CustomerSearchRequest searchRequest, Guid userId)
        {
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    IQueryable<Entity> accounts = serviceContext.CreateQuery(EntityNames.Account)
                                                    .Where(ac => ac.GetAttributeValue<EntityReference>(MetadataAccount.BETREIBEREPMEXTENSIONPORTALUSER) != null
                                                              && ac.GetAttributeValue<EntityReference>(MetadataAccount.BETREIBEREPMEXTENSIONPORTALUSER).Id == userId);

                    int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
                    int toRow = searchRequest.PageSize;
                    bool searchSpecified = !string.IsNullOrEmpty(searchRequest.Param);
                    Func<Entity, bool> expression =
                        s => (
                            searchSpecified &&
                            (s.Contains(MetadataAccount.NAME) &&
                             !string.IsNullOrEmpty(s.GetAttributeValue<string>(MetadataAccount.NAME)) &&
                             s.GetAttributeValue<string>(MetadataAccount.NAME)
                                 .ToLower()
                                 .Contains(searchRequest.Param.ToLower()))
                            || !searchSpecified);

                    IEnumerable<Entity> oList =
                    searchRequest.IsAsc ?
                    accounts.Where(expression).OrderBy(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
                    accounts.Where(expression).OrderByDescending(userActivityClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();

                    int recordCount = accounts.Count(expression);

                    return new CustomerResponse { Customers = oList.Select(WrapAccountIntoCrmAccount).ToList(), TotalCount = recordCount, UserTotalCount = recordCount };
                }
            }
        }


        public List<CrmAccount> GetBeitreibersByAccountId(Guid crmAccountId)
        {
            List<CrmAccount> Beitreibers = new List<CrmAccount>();

            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    Beitreibers = this.GetBeitreibersByAccountId(serviceContext, crmAccountId);
                }
            }

            return Beitreibers;
        }

        public List<CrmAccount> GetBeitreibersByAccountId(OrganizationServiceContext serviceContext, Guid crmAccountId)
        {

            List<MeteringPoint> meteringPoints = this.GetMeteringPointsByAccountId(crmAccountId);
            List<MeteringPoint> uniqueBeitreiberMeteringPoints = meteringPoints.GroupBy(mp => mp.BetreiberId)
                                                                               .Select(grp => grp.First())
                                                                               .ToList<MeteringPoint>();

            List<CrmAccount> beitreibers = new List<CrmAccount>();
            foreach (MeteringPoint beitreiberMeteringPoint in uniqueBeitreiberMeteringPoints)
            {
                if (beitreiberMeteringPoint.BetreiberId != Guid.Empty)
                {
                    beitreibers.Add(this.GetAccountById(beitreiberMeteringPoint.BetreiberId));
                }
            }

            return beitreibers;
        }
        #endregion ""

        #region "MeteringPoint"

        public MeteringPoint GetMeteringPointById(Guid crmAccountId)
        {
            MeteringPoint crmAccount = new MeteringPoint();
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    crmAccount = GetMeteringPointById(serviceContext, crmAccountId);
                }
            }

            return crmAccount;
        }

        private MeteringPoint GetMeteringPointById(OrganizationServiceContext serviceContext, Guid crmAccountId)
        {
            var accounts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt).Where(e => ((Guid?)e[MetadataDZählpunkt.METERING_POINT_ID]) == crmAccountId);
            var result = this.GetMeteringPointsFromEntityCollection(serviceContext, accounts);
            return result.FirstOrDefault();

        }
        public List<MeteringPoint> GetMeteringPoints()
        {
            List<MeteringPoint> meteringPoints = new List<MeteringPoint>();

            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    IQueryable<Entity> zahplunkts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt);
                    meteringPoints = this.GetMeteringPoints(serviceContext);
                }
            }

            return meteringPoints;
        }

        public List<MeteringPoint> GetMeteringPoints(OrganizationServiceContext serviceContext)
        {
            IQueryable<Entity> zahplunkts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt);
            return this.GetMeteringPointsFromEntityCollection(serviceContext, zahplunkts);
        }

        public List<MeteringPoint> GetMeteringPointsByAccountId(Guid crmAccountId)
        {
            List<MeteringPoint> meteringPoints = new List<MeteringPoint>();

            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    var zahplunkts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt).Where(za => za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.ACCOUNT).Id == crmAccountId);
                    meteringPoints = GetMeteringPointsFromEntityCollection(serviceContext, zahplunkts);
                }
            }

            return meteringPoints;
        }

        public MeteringPoint GetMeteringPointByCode(string meteringPointCode)
        {
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    var zahplunkts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt).Where(za => za.GetAttributeValue<string>(MetadataDZählpunkt.ZAHLPUNKTBEZEICHNER) == meteringPointCode);
                    List<MeteringPoint> meteringPoints = GetMeteringPointsFromEntityCollection(serviceContext, zahplunkts);
                    if(meteringPoints != null && meteringPoints.Count == 1)
                    {
                        return meteringPoints[0];
                    }
                }
            }

            return null;
        }

        #region "Beitreibers Metering Points"
        public MeteringPointResponse GetBeitreiberMetringPoints(MeteringPointSearchRequest request)
        {

            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    return GetBeitreiberMetringPoints(serviceContext, request);
                }
            }
        }

        private readonly Dictionary<MeteringPointColumnBy, Func<Entity, object>> meteringPointClause =
                 new Dictionary<MeteringPointColumnBy, Func<Entity, object>>
                    {
                        {MeteringPointColumnBy.Zählpunktbezeichner, c => c.Attributes[MetadataDZählpunkt.ZAHLPUNKTBEZEICHNER]},
                        {MeteringPointColumnBy.Kurzbezeichnung, c => c.Attributes[MetadataDZählpunkt.KURZEEZEICHNUNG]},
                        {MeteringPointColumnBy.Anlagentyp, c => c.Attributes[MetadataDZählpunkt.ANLAGENTYP]},
                        {MeteringPointColumnBy.Strasse, c => c.Attributes[MetadataDZählpunkt.STRASSE]},
                        {MeteringPointColumnBy.PLZ, c => c.Attributes[MetadataDZählpunkt.PLZ]},
                        {MeteringPointColumnBy.Ort, c => c.Attributes[MetadataDZählpunkt.ORT]},
                        {MeteringPointColumnBy.Datenversand, c => c.Attributes[MetadataDZählpunkt.DATENVERSANDAKTIV]},
                        {MeteringPointColumnBy.Zählverfahren, c => c.Attributes[MetadataDZählpunkt.ZAHLVERFAHREN]},
                        {MeteringPointColumnBy.Messung, c => c.Attributes[MetadataDZählpunkt.UMESSUNG]}
                    };

        public MeteringPointResponse GetBeitreiberMetringPoints(OrganizationServiceContext serviceContext, MeteringPointSearchRequest searchRequest)
        {
            int fromRow = (searchRequest.PageNo - 1) * searchRequest.PageSize;
            bool searchSpecified = !string.IsNullOrEmpty(searchRequest.Param);
            int toRow = searchRequest.PageSize;

            Func<Entity, bool> expression =
                za => ((za.Contains(MetadataDZählpunkt.ACCOUNT)
                        && za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.ACCOUNT) != null
                        && za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.ACCOUNT).Id == searchRequest.CustomerId))
                        && (za.Contains(MetadataDZählpunkt.BETREIBER)
                        && (za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.BETREIBER).Id != null
                        && za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.BETREIBER).Id == searchRequest.BetrieberId))
                        && (searchRequest.Param != null
                        && za.Contains(MetadataDZählpunkt.KURZEEZEICHNUNG)
                        && (za.GetAttributeValue<string>(MetadataDZählpunkt.KURZEEZEICHNUNG).IndexOf(searchRequest.Param, StringComparison.OrdinalIgnoreCase) >= 0)
                        && searchSpecified || !searchSpecified);
            IQueryable<Entity> zahplunkts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt);
            zahplunkts.Where(expression);
            IEnumerable<Entity> oList =
            searchRequest.IsAsc ?
            zahplunkts.Where(expression).OrderBy(meteringPointClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList() :
            zahplunkts.Where(expression).OrderByDescending(meteringPointClause[searchRequest.OrderBy]).Skip(fromRow).Take(toRow).ToList();
            var meteringPoints = this.GetMeteringPointsFromEntityCollection(serviceContext, oList);
            return new MeteringPointResponse { MeteringPoints = meteringPoints, TotalCount = oList.Where(expression).ToList().Count };
        }

        private List<MeteringPoint> GetMeteringPointsFromEntityCollection(OrganizationServiceContext serviceContext, IEnumerable<Entity> zahplunkts)
        {
            List<MeteringPoint> meteringPoints = new List<MeteringPoint>();
            foreach (Entity zahplunkt in zahplunkts)
            {
                MeteringPoint meteringPoint = new MeteringPoint();

                if (zahplunkt.Contains(MetadataDZählpunkt.METERING_POINT_ID))
                {
                    meteringPoint.Id = zahplunkt.GetAttributeValue<Guid>(MetadataDZählpunkt.METERING_POINT_ID);
                }
                if (zahplunkt.Contains(MetadataDZählpunkt.NAME))
                {
                    meteringPoint.Name = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.NAME);
                }
                if (zahplunkt.Contains(MetadataDZählpunkt.ACCOUNT))
                {
                    EntityReference linkedAccount = zahplunkt.GetAttributeValue<EntityReference>(MetadataDZählpunkt.ACCOUNT);
                    meteringPoint.CrmAccountId = linkedAccount.Id;
                    meteringPoint.CrmAccountName = linkedAccount.Name;

                    //GetBeitreibersByAccountId(linkedAccount.Id);
                }
                if (zahplunkt.Contains(MetadataDZählpunkt.BETREIBER))
                {
                    EntityReference linkedBetreiber = zahplunkt.GetAttributeValue<EntityReference>(MetadataDZählpunkt.BETREIBER);
                    meteringPoint.BetreiberId = linkedBetreiber.Id;
                    meteringPoint.BetreiberName = linkedBetreiber.Name;
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
                if (zahplunkt.Contains(MetadataDZählpunkt.STRASSE))
                {
                    meteringPoint.Strasse = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.STRASSE);
                }
                if (zahplunkt.Contains(MetadataDZählpunkt.ORT))
                {
                    meteringPoint.Ort = zahplunkt.GetAttributeValue<string>(MetadataDZählpunkt.ORT);
                }
                if (zahplunkt.Contains(MetadataDZählpunkt.DATENVERSANDAKTIV))
                {
                    meteringPoint.DatenversandAktiv = zahplunkt.GetAttributeValue<bool>(MetadataDZählpunkt.DATENVERSANDAKTIV) ? "Yes" : "No";
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
                    this.SetSetSpannungsebene(zahplunkt.GetAttributeValue<OptionSetValue>(MetadataDZählpunkt.UMESSUNG).Value,
                                        out code, out value);

                    meteringPoint.UMessungCode = code;
                    meteringPoint.UMessungValue = value;
                }
                if (zahplunkt.Contains(MetadataDZählpunkt.UENTNAHME))
                {
                    int code; string value;
                    this.SetSetSpannungsebene(zahplunkt.GetAttributeValue<OptionSetValue>(MetadataDZählpunkt.UENTNAHME).Value,
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

                #region "Threshold Values"
                //Get the threshold for this metering point
                IQueryable<Entity> grenzwerts = serviceContext.CreateQuery(EntityNames.Grenzwert).Where(g => g.GetAttributeValue<EntityReference>(MetadataGrenzwert.GrenzwerteZPID).Id == zahplunkt.Id);
                List<MeteringPointThreshold> meteringPointThresholds = new List<MeteringPointThreshold>();
                meteringPoint.MeteringCodeThresholds = new List<MeteringPointThreshold>();
                if (grenzwerts != null)
                {
                    Entity grenzwert = grenzwerts.ToList().FirstOrDefault();
                    if (grenzwert != null)
                    {
                        #region "System Threshold Values"
                        MeteringPointThreshold meteringPointThreshlodSystem = new MeteringPointThreshold { Type = MeteringPointThresholdType.System, GrenzwertType = "Grenzwert System", IsActive = true};

                        if (grenzwert.Contains(MetadataGrenzwert.Grenze))
                        {
                            meteringPointThreshlodSystem.GrenzwertBezeichner = String.Format("{0:F2}", grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.Grenze));
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwerteId))
                        {
                            meteringPointThreshlodSystem.Id = grenzwert.GetAttributeValue<Guid>(MetadataGrenzwert.GrenzwerteId);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GültigAb))
                        {
                            meteringPointThreshlodSystem.GultingAb = grenzwert.GetAttributeValue<DateTime>(MetadataGrenzwert.GültigAb);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMaxSystem))
                        {
                            meteringPointThreshlodSystem.MaximaGlobal = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMaxSystem);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMinSystem))
                        {
                            meteringPointThreshlodSystem.MinimaGlobal = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMinSystem);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMaxSystem))
                        {
                            meteringPointThreshlodSystem.MaximaSommer = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMaxSystem);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMinSystem))
                        {
                            meteringPointThreshlodSystem.MinimaSommer = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMinSystem);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMaxSystem))
                        {
                            meteringPointThreshlodSystem.MaximaWinter = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMaxSystem);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMinSystem))
                        {
                            meteringPointThreshlodSystem.MinimaWinter = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMinSystem);
                        }
                        #endregion "System Threshold Values"

                        #region "User Threshold Values"
                        MeteringPointThreshold meteringPointThreshlodUser = new MeteringPointThreshold { Type = MeteringPointThresholdType.User, GrenzwertType = "Grenzwert Benutzer",IsActive = false};
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwerteId))
                        {
                            meteringPointThreshlodUser.Id = grenzwert.GetAttributeValue<Guid>(MetadataGrenzwert.GrenzwerteId);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.Grenze))
                        {
                            meteringPointThreshlodUser.GrenzwertBezeichner = String.Format("{0:F2}", grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.Grenze));
                            
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GültigAb))
                        {
                            meteringPointThreshlodUser.GultingAb = grenzwert.GetAttributeValue<DateTime>(MetadataGrenzwert.GültigAb);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMaxUser))
                        {
                            meteringPointThreshlodUser.MaximaGlobal = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMaxUser);
                            meteringPointThreshlodUser.IsActive = true;
                            meteringPointThreshlodSystem.IsActive = false;
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMinUser))
                        {
                            meteringPointThreshlodUser.MinimaGlobal = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMinUser);
                            meteringPointThreshlodUser.IsActive = true;
                            meteringPointThreshlodSystem.IsActive = false;
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMaxUser))
                        {
                            meteringPointThreshlodUser.MaximaSommer = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMaxUser);
                            meteringPointThreshlodUser.IsActive = true;
                            meteringPointThreshlodSystem.IsActive = false;
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMinUser))
                        {
                            meteringPointThreshlodUser.MinimaSommer = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMinUser);
                            meteringPointThreshlodUser.IsActive = true;
                            meteringPointThreshlodSystem.IsActive = false;
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMaxUser))
                        {
                            meteringPointThreshlodUser.MaximaWinter = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMaxUser);
                            meteringPointThreshlodUser.IsActive = true;
                            meteringPointThreshlodSystem.IsActive = false;
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMinUser))
                        {
                            meteringPointThreshlodUser.MinimaWinter = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMinUser);
                            meteringPointThreshlodUser.IsActive = true;
                            meteringPointThreshlodSystem.IsActive = false;
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.Seasonal))
                        {
                            meteringPointThreshlodUser.SaisonalitatAnwenden = grenzwert.GetAttributeValue<OptionSetValue>(MetadataGrenzwert.Seasonal).Value == 100000000 ? true : false;
                        }
                        #endregion "User Threshold Values"

                        #region "Email Reports"
                        if (grenzwert.Contains(MetadataGrenzwert.EMailBerichte))
                        {
                            meteringPointThreshlodSystem.EMailBerichte = grenzwert.GetAttributeValue<OptionSetValue>(MetadataGrenzwert.EMailBerichte).Value;
                            meteringPointThreshlodUser.EMailBerichte = grenzwert.GetAttributeValue<OptionSetValue>(MetadataGrenzwert.EMailBerichte).Value;
                        }
                        #endregion "Email Reports" 
                       
                        #region "Empfaenger"
                        if (grenzwert.Contains(MetadataGrenzwert.Empfaenger1))
                        {
                            meteringPointThreshlodSystem.Empfaenger1 = grenzwert.GetAttributeValue<string>(MetadataGrenzwert.Empfaenger1);
                            meteringPointThreshlodUser.Empfaenger1 = grenzwert.GetAttributeValue<string>(MetadataGrenzwert.Empfaenger1);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.Empfaenger2))
                        {
                            meteringPointThreshlodSystem.Empfaenger2 = grenzwert.GetAttributeValue<string>(MetadataGrenzwert.Empfaenger2);
                            meteringPointThreshlodUser.Empfaenger2 = grenzwert.GetAttributeValue<string>(MetadataGrenzwert.Empfaenger2);
                        }
                        if (grenzwert.Contains(MetadataGrenzwert.Empfaenger3))
                        {
                            meteringPointThreshlodSystem.Empfaenger3 = grenzwert.GetAttributeValue<string>(MetadataGrenzwert.Empfaenger3);
                            meteringPointThreshlodUser.Empfaenger3 = grenzwert.GetAttributeValue<string>(MetadataGrenzwert.Empfaenger3);
                        }
                        #endregion "Empfaenger"

                        meteringPointThresholds.Add(meteringPointThreshlodSystem);
                        meteringPointThresholds.Add(meteringPointThreshlodUser);

                        meteringPoint.MeteringCodeThresholds = meteringPointThresholds;
                    }
                
                
                }
                #endregion "Threshold Values"

                meteringPoint.KundenspezifikationZp = this.GetKundenspezifikationZPByMeteringPointId(meteringPoint.Id);

                meteringPoints.Add(meteringPoint);
            }

            return meteringPoints;
        }
        #endregion "Beitreibers Metering Points"

        private void SetSetSpannungsebene(int caseValue, out int code, out string value)
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

        public void UpdateMeteringPointSpecificationsAndThreashold(MeteringPoint meteringPoint)
        {
            if (meteringPoint != null)
            {
                using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
                {                    
                    if (meteringPoint.MeteringCodeThresholds != null && meteringPoint.MeteringCodeThresholds.Count() > 0)
                    {
                        MeteringPointThreshold userThreshold = meteringPoint.MeteringCodeThresholds.FirstOrDefault(t => t.Type == MeteringPointThresholdType.User);
                        if (userThreshold != null)
                        {
                            Entity thresholdToUpdate = new Entity(EntityNames.Grenzwert);
                            thresholdToUpdate.Id = userThreshold.Id;

                            thresholdToUpdate.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.Empfaenger1, userThreshold.Empfaenger1));
                            thresholdToUpdate.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.Empfaenger2, userThreshold.Empfaenger2));
                            thresholdToUpdate.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.Empfaenger3, userThreshold.Empfaenger3));

                            serviceProxy.Update(thresholdToUpdate);
                        }

                        KundenspezifikationZp specification = meteringPoint.KundenspezifikationZp;
                        if (specification != null)
                        {
                            Entity specificationToTupdate = new Entity(EntityNames.Kundenspezifikation_ZP);
                            specificationToTupdate.Id = specification.Id;

                            if (specification.Gesamtflache > 0) specificationToTupdate.Attributes.Add(new KeyValuePair<string, object>(MetadataKundenspezifikation_ZP.Gesamtflache_Flaechentyp1, specification.Gesamtflache));
                            if (specification.Nebenflache > 0) specificationToTupdate.Attributes.Add(new KeyValuePair<string, object>(MetadataKundenspezifikation_ZP.Nebenflache_Flaechentyp2, specification.Nebenflache));
                            if (specification.BeheizteFlache> 0) specificationToTupdate.Attributes.Add(new KeyValuePair<string, object>(MetadataKundenspezifikation_ZP.Beheizte_Flache_Flaechentyp3, specification.BeheizteFlache));
                            if (specification.UnbeheizteFlache > 0) specificationToTupdate.Attributes.Add(new KeyValuePair<string, object>(MetadataKundenspezifikation_ZP.Unbeheizte_Flache_Flaechentyp4, specification.UnbeheizteFlache));
                            if (specification.SonstigeFlachen > 0) specificationToTupdate.Attributes.Add(new KeyValuePair<string, object>(MetadataKundenspezifikation_ZP.Sonstige_Flachen_Flaechentyp5, specification.SonstigeFlachen));

                            specificationToTupdate.Attributes.Add(new KeyValuePair<string, object>(MetadataKundenspezifikation_ZP.Notizfeld_Freitextfeld1, specification.Notizfeld));

                            serviceProxy.Update(specificationToTupdate);
                        }
                    }
                }
            }
        }

        #endregion "MeteringPoint"

        #region "Threshold"

        public MeteringPointThreshold GetThresholdById(Guid thresholdId)
        {
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    IQueryable<Entity> grenzwerts = serviceContext.CreateQuery(EntityNames.Grenzwert).Where(g => g.GetAttributeValue<Guid>(MetadataGrenzwert.Id) == thresholdId);
                    if (grenzwerts != null && grenzwerts.ToList().Count() >= 1)
                    {
                        Entity grenzwert = grenzwerts.ToArray().FirstOrDefault();

                        if (grenzwert != null)
                        {
                            MeteringPointThreshold meteringPointThreshlodUser = new MeteringPointThreshold { Type = MeteringPointThresholdType.User };
                            if (grenzwert.Contains(MetadataGrenzwert.GrenzwerteId))
                            {
                                meteringPointThreshlodUser.Id = grenzwert.GetAttributeValue<Guid>(MetadataGrenzwert.GrenzwerteId);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.Grenze))
                            {
                                meteringPointThreshlodUser.GrenzwertBezeichner = String.Format("{0:F2}", grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.Grenze));
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.GültigAb))
                            {
                                meteringPointThreshlodUser.GultingAb = grenzwert.GetAttributeValue<DateTime>(MetadataGrenzwert.GültigAb);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMaxUser))
                            {
                                meteringPointThreshlodUser.MaximaGlobal = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMaxUser);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.GrenzwertMinUser))
                            {
                                meteringPointThreshlodUser.MinimaGlobal = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertMinUser);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMaxUser))
                            {
                                meteringPointThreshlodUser.MaximaSommer = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMaxUser);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.GrenzwertSommerMinUser))
                            {
                                meteringPointThreshlodUser.MinimaSommer = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertSommerMinUser);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMaxUser))
                            {
                                meteringPointThreshlodUser.MaximaWinter = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMaxUser);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.GrenzwertWinterMinUser))
                            {
                                meteringPointThreshlodUser.MinimaWinter = grenzwert.GetAttributeValue<decimal>(MetadataGrenzwert.GrenzwertWinterMinUser);
                            }
                            if (grenzwert.Contains(MetadataGrenzwert.Seasonal))
                            {
                                meteringPointThreshlodUser.SaisonalitatAnwenden = grenzwert.GetAttributeValue<OptionSetValue>(MetadataGrenzwert.Seasonal).Value == 100000000? true: false;
                            }

                            return meteringPointThreshlodUser;
                        }
                    }

                }
            }

            return null;
        }

        public void UpdateMeteringPointThreshold(MeteringPointThreshold threshold)
        {
            if (threshold.Id != null && threshold.Id != Guid.Empty)
            {
                using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
                {
                    using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                    {
                        Entity crmThreshold = new Entity(EntityNames.Grenzwert);
                        crmThreshold.Id = threshold.Id;
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GültigAb, threshold.GultingAb));                        
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.Seasonal, new OptionSetValue(threshold.SaisonalitatAnwenden? 100000000: 100000001)));                        
                        if (threshold.MaximaGlobal > 0) crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertMaxUser, (Object)threshold.MaximaGlobal));
                        if (threshold.MinimaGlobal > 0) crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertMinUser, (Object)threshold.MinimaGlobal));
                        if (threshold.MaximaSommer > 0) crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertSommerMaxUser, (Object)threshold.MaximaSommer));
                        if (threshold.MinimaSommer > 0) crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertSommerMinUser, (Object)threshold.MinimaSommer));
                        if (threshold.MaximaWinter > 0) crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertWinterMaxUser, (Object)threshold.MaximaWinter));
                        if (threshold.MinimaWinter > 0) crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertWinterMinUser, (Object)threshold.MinimaWinter));
                        if (!serviceContext.IsAttached(crmThreshold))
                        {
                            serviceContext.Attach(crmThreshold);
                        }
                        serviceContext.UpdateObject(crmThreshold);
                        serviceContext.SaveChanges();
                    }
                }
            }
        }

        public void SetThresholdReport(Guid thresholdId, MetadataGrenzwert.OpSetReport report, string Empfaenger1, string Empfaenger2, string Empfaenger3)
        {
            if (thresholdId != null && thresholdId != Guid.Empty)
            {
                using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
                {
                    Entity crmThreshold = new Entity(EntityNames.Grenzwert);
                    crmThreshold.Id = thresholdId;
                    crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.EMailBerichte, (Object) new OptionSetValue((int)report)));
                    crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.Empfaenger1, Empfaenger1));
                    crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.Empfaenger2, Empfaenger2));
                    crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.Empfaenger3, Empfaenger3));

                    serviceProxy.Update(crmThreshold);
                }
            }
        }

        #endregion "Threshold"

        #region "Kundenspezifikation_ZP"
        public KundenspezifikationZp GetKundenspezifikationZPByMeteringPointId(Guid meteringPointId)
        {
            KundenspezifikationZp crmKundenspezifikationZP = new KundenspezifikationZp();
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    crmKundenspezifikationZP = GetKundenspezifikationZPByMeteringPointId(serviceContext, meteringPointId);
                }
            }

            return crmKundenspezifikationZP;
        }

        private KundenspezifikationZp GetKundenspezifikationZPByMeteringPointId(OrganizationServiceContext serviceContext, Guid meteringPointId)
        {
            var crmKundenspezifikationZPs = serviceContext.CreateQuery(EntityNames.Kundenspezifikation_ZP).Where(e => ((Guid?)e[MetadataKundenspezifikation_ZP.Zahlpunkt]) == meteringPointId);
            var result = GetKundenspezifikationZPFromEntityCollection(serviceContext, crmKundenspezifikationZPs);
            return result.FirstOrDefault();
        }

        private List<KundenspezifikationZp> GetKundenspezifikationZPFromEntityCollection(OrganizationServiceContext serviceContext, IEnumerable<Entity> crmKundenspezifikationZPs)
        {
            List<KundenspezifikationZp> kundenspezifikationZPs = new List<KundenspezifikationZp>();
            foreach (Entity crmKundenspezifikationZP in crmKundenspezifikationZPs)
            {
                KundenspezifikationZp kundenspezifikation_ZP = new KundenspezifikationZp();
                kundenspezifikation_ZP.Id = crmKundenspezifikationZP.Id;

                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.KlimatisierungAktiv))
                {
                    string value = String.Empty;
                    int code = -1;
                    switch (crmKundenspezifikationZP.GetAttributeValue<OptionSetValue>(MetadataKundenspezifikation_ZP.KlimatisierungAktiv).Value)
                    {
                        case (int)MetadataKundenspezifikation_ZP.OpSetKlimatisierungAktiv.JA:
                            code = (int)MetadataKundenspezifikation_ZP.OpSetKlimatisierungAktiv.JA;
                            value = "Ja";
                            break;
                        case (int)MetadataKundenspezifikation_ZP.OpSetKlimatisierungAktiv.NEIN:
                            code = (int)MetadataKundenspezifikation_ZP.OpSetKlimatisierungAktiv.NEIN;
                            value = "Nein";
                            break;
                        case (int)MetadataKundenspezifikation_ZP.OpSetKlimatisierungAktiv.UNBEKANNT:
                            code = (int)MetadataKundenspezifikation_ZP.OpSetKlimatisierungAktiv.UNBEKANNT;
                            value = "Unbekannt";
                            break;
                    }
                    kundenspezifikation_ZP.KlimatisierungAktivCode = code;
                    kundenspezifikation_ZP.KlimatisierungAktivValue = value;
                }
                
                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.Beheizte_Flache_Flaechentyp3))
                {
                    kundenspezifikation_ZP.BeheizteFlache = crmKundenspezifikationZP.GetAttributeValue<int>(MetadataKundenspezifikation_ZP.Beheizte_Flache_Flaechentyp3);
                }
                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.Gesamtflache_Flaechentyp1))
                {
                    kundenspezifikation_ZP.Gesamtflache = crmKundenspezifikationZP.GetAttributeValue<int>(MetadataKundenspezifikation_ZP.Gesamtflache_Flaechentyp1);
                }
                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.Nebenflache_Flaechentyp2))
                {
                    kundenspezifikation_ZP.Nebenflache = crmKundenspezifikationZP.GetAttributeValue<int>(MetadataKundenspezifikation_ZP.Nebenflache_Flaechentyp2);
                }                
                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.Sonstige_Flachen_Flaechentyp5))
                {
                    kundenspezifikation_ZP.SonstigeFlachen = crmKundenspezifikationZP.GetAttributeValue<int>(MetadataKundenspezifikation_ZP.Sonstige_Flachen_Flaechentyp5);
                }
                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.Unbeheizte_Flache_Flaechentyp4))
                {
                    kundenspezifikation_ZP.UnbeheizteFlache = crmKundenspezifikationZP.GetAttributeValue<int>(MetadataKundenspezifikation_ZP.Unbeheizte_Flache_Flaechentyp4);
                }

                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.Zahlpunkt))
                {
                    EntityReference linkedMeteringPoint = crmKundenspezifikationZP.GetAttributeValue<EntityReference>(MetadataKundenspezifikation_ZP.Zahlpunkt);
                    kundenspezifikation_ZP.ZahlpunktId = linkedMeteringPoint.Id;
                    kundenspezifikation_ZP.ZahlpunktName = linkedMeteringPoint.Name;                        
                }

                if (crmKundenspezifikationZP.Contains(MetadataKundenspezifikation_ZP.Notizfeld_Freitextfeld1))
                {
                    kundenspezifikation_ZP.Notizfeld = crmKundenspezifikationZP.GetAttributeValue<string>(MetadataKundenspezifikation_ZP.Notizfeld_Freitextfeld1);
                }                
                
                kundenspezifikationZPs.Add(kundenspezifikation_ZP);
            }

            return kundenspezifikationZPs;
        }
        #endregion "Kundenspezifikation_ZP"

        #region Dynamics CRM Connection
        private static OrganizationServiceProxy GetProxyService()
        {
            IServiceManagement<IDiscoveryService> serviceManagement = DynamicsCrmService.GetDiscoveryService("http://10.10.30.6:5555");
            IServiceManagement<IOrganizationService> orgServiceManagement = DynamicsCrmService.GetOrganisationService("http://10.10.30.6:5555", "crm-epm");
            AuthenticationCredentials authenticationCredentials = GetCredentials(serviceManagement.AuthenticationType, "Dev0001", "London123", "ENOMOS");

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
