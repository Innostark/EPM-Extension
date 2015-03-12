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
        #region "Account"
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
        private readonly Dictionary<CustomerColumnBy, Func<Entity, object>> userActivityClause =
                  new Dictionary<CustomerColumnBy, Func<Entity, object>>
                    {
                        {CustomerColumnBy.Name, c => c.Attributes[MetadataAccount.NAME]},
                        {CustomerColumnBy.Number, c => c.Attributes[MetadataAccount.KUNDENNUMMER]},
                        {CustomerColumnBy.Address, c => c.Attributes[MetadataAccount.STRASSE]},
                        {CustomerColumnBy.ZipCode, c => c.Attributes[MetadataAccount.PLZ]},
                        {CustomerColumnBy.City, c => c.Attributes[MetadataAccount.ORT]}
                    };
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
                if ( beitreiberMeteringPoint.BetreiberId != Guid.Empty)
                {
                    beitreibers.Add(this.GetAccountById(beitreiberMeteringPoint.BetreiberId));
                }
            }

            return beitreibers;
        }
        #endregion ""

        #region "MeteringPoint"

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
                za => ( za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.ACCOUNT) != null
                        && za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.ACCOUNT).Id == searchRequest.CustomerId
                        && za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.BETREIBER).Id != null
                        && za.GetAttributeValue<EntityReference>(MetadataDZählpunkt.BETREIBER).Id == searchRequest.BetrieberId
                        && (za.GetAttributeValue<string>(MetadataDZählpunkt.KURZEEZEICHNUNG).IndexOf(searchRequest.Param, StringComparison.OrdinalIgnoreCase) >= 0 
                            && searchSpecified || !searchSpecified)
                     );
            IQueryable<Entity> zahplunkts = serviceContext.CreateQuery(EntityNames.D_Zählpunkt);
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
                        MeteringPointThreshold meteringPointThreshlodSystem = new MeteringPointThreshold { Type = MeteringPointThresholdType.System };

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
                        #endregion "System Threshold Values"

                        #region "User Threshold Values"
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
                        #endregion "User Threshold Values"

                        meteringPoint.MeteringCodeThresholds = meteringPointThresholds;
                    }
                #endregion "Threshold Values"
                }
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
        #endregion "MeteringPoint"

        #region "Threshold"

        public MeteringPointThreshold GetThresholdById(Guid thresholdId)
        {
            using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
            {
                using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                {
                    IQueryable<Entity> grenzwerts = serviceContext.CreateQuery(EntityNames.Grenzwert).Where(g => g.GetAttributeValue<Guid>(MetadataGrenzwert.Id) == thresholdId);
                    if(grenzwerts != null && grenzwerts.ToList().Count() >= 1)
                    {
                        Entity grenzwert = grenzwerts.ToArray().FirstOrDefault();

                        if(grenzwert != null)
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

                            return meteringPointThreshlodUser;
                        }
                    }                   

                }
            }

            return null;
        }

        internal void UpdateMeteringPointThreshold(MeteringPointThreshold threshold)
        {
            if (threshold.Id != null && threshold.Id != Guid.Empty)
            {
                using (OrganizationServiceProxy serviceProxy = DynamicsCrmService.GetProxyService())
                {
                    using (OrganizationServiceContext serviceContext = new OrganizationServiceContext(serviceProxy))
                    {
                        Entity crmThreshold = new Entity(EntityNames.Grenzwert);
                        crmThreshold.Id = threshold.Id;
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertMaxUser, (Object)threshold.MaximaGlobal));
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertMinUser, (Object)threshold.MinimaGlobal));
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertSommerMaxUser, (Object)threshold.MaximaSommer));
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertSommerMinUser, (Object)threshold.MinimaSommer));
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertWinterMaxUser, (Object)threshold.MaximaWinter));
                        crmThreshold.Attributes.Add(new KeyValuePair<string, object>(MetadataGrenzwert.GrenzwertWinterMinUser, (Object)threshold.MinimaWinter));
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

        #endregion "Threshold"

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
