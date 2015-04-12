using System;
namespace EPM.Extension.Model
{
    public class Kundenspezifikation_ZP
    {
        public Guid Id { get; set; }

        public int Gesamtflache { get; set; }
        public int Nebenflache { get; set; }
        public int Beheizte_Flache { get; set; }
        public int Unbeheizte_Flache { get; set; }
        public int Sonstige_Flachen { get; set; }
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
