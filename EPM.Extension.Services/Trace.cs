using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPM.Extension.Services
{
    public static class Trace
    {
        public static void LogError(Exception ex)
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine("Date time: ");
            log.AppendLine(DateTime.Now.ToLongTimeString());

            log.AppendLine("Stack Trace: ");
            log.AppendLine(ex.StackTrace);
            log.AppendLine("Exception Message: ");
            log.AppendLine(ex.Message);

            System.Diagnostics.Trace.WriteLine(log);
            System.Diagnostics.Trace.Flush();
        }
    }
}
