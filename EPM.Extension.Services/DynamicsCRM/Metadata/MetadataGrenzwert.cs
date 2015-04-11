using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Services.DynamicsCRM.Metadata
{
    public class MetadataGrenzwert
    {
        public const string GrenzwerteZPID = "new_grenzwertezpidid";
        public const string Grenze = "new_grenze";
        public const string GültigAb = "new_gltigab";
        public const string GrenzwerteId = "new_grenzwerteid";
        public const string Id = "new_grenzwerteid";
        public const string Seasonal = "new_saisonjanein";
        public const string EMailBerichte = "new_emailreports";

       #region System
		public const string GrenzwertMaxSystem = "new_grenzwertmaxsystem";
        public const string GrenzwertMinSystem = "new_grenzwertminsystem";
        public const string GrenzwertSommerMaxSystem = "new_grenzwertsommermaxsystem";
        public const string GrenzwertSommerMinSystem = "new_grenzwertsommerminsystem";
        public const string GrenzwertWinterMaxSystem = "new_grenzwertwintermaxsystem";
        public const string GrenzwertWinterMinSystem = "new_grenzwertwinterminsystem";
	#endregion System

       #region User
		 public const string GrenzwertMaxUser = "new_grenzwertmaxuser";
        public const string GrenzwertMinUser = "new_grenzwertminuser";
        public const string GrenzwertSommerMaxUser = "new_grenzwertsommermaxuser";
        public const string GrenzwertSommerMinUser = "new_grenzwertsommerminuser";
        public const string GrenzwertWinterMaxUser = "new_grenzwertwintermaxuser";
        public const string GrenzwertWinterMinUser = "new_grenzwertwinterminuser";
	#endregion User

        public enum OpSetSeasonal
        {
            Ja = 100000000,
            Nein = 100000001
        }

        public enum OpSetReport
        {
            Aktiv = 100000000,
            NeinAktiv = 100000001
        }

    }
}
