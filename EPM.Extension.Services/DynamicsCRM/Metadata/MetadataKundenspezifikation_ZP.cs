namespace EPM.Extension.Services.DynamicsCRM.Metadata
{
    public class MetadataKundenspezifikation_ZP
    {
        public const string KlimatisierungAktiv = "new_klimatisierungaktiv"; //OptionSet
        public const string Gesamtflache_Flaechentyp1 = "new_flaechentyp1"; //Whole number
        public const string Nebenflache_Flaechentyp2 = "new_flaechentyp2"; //Whole number
        public const string Beheizte_Flache_Flaechentyp3 = "new_flaechentyp3"; //Whole number
        public const string Unbeheizte_Flache_Flaechentyp4 = "new_flaechentyp4"; //Whole number
        public const string Sonstige_Flachen_Flaechentyp5 = "new_flaechentyp5"; //Whole number
        public const string Notizfeld_Freitextfeld1  = "new_freitextfeld1"; //MLOT 20000
        public const string Zahlpunkt = "new_zhlpunkt";

        public enum OpSetKlimatisierungAktiv
        {
            JA = 100000000,
            NEIN = 100000001,
            UNBEKANNT = 100000002
        }
    }
}
