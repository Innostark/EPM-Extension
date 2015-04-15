using System;
namespace EPM.Extension.Model
{
    public class KundenspezifikationZp
    {
        public Guid Id { get; set; }

        public int Gesamtflache { get; set; }
        public int Nebenflache { get; set; }
        public int BeheizteFlache { get; set; }
        public int UnbeheizteFlache { get; set; }
        public int SonstigeFlachen { get; set; }
        public string Notizfeld { get; set; }

        #region Links
        public Guid ZahlpunktId { get; set; }
        public string ZahlpunktName { get; set; }
        #endregion Links

        #region OptionSets
        public string KlimatisierungAktivValue { get; set; }
        public int KlimatisierungAktivCode { get; set; }
        #endregion OptionSets
    }

}
