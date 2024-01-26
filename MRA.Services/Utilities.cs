using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public static class Utilities
    {
        public static string FormattedDate(string Date)
        {
            if (String.IsNullOrEmpty(Date))
                return "";

            DateTime date = DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            //CultureInfo currentCulture = CultureInfo.CurrentCulture;
            var cultureInfo = CultureInfo.GetCultureInfo("es-ES");
            return date.ToString("dd MMMM yyyy", cultureInfo);
            
        }

        public static string FormattedDateMini(string Date)
        {
            if (String.IsNullOrEmpty(Date))
                return "";

            DateTime date = DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            //CultureInfo currentCulture = CultureInfo.CurrentCulture;
            var cultureInfo = CultureInfo.GetCultureInfo("es-ES");
            return date.ToString("MMMM yy", cultureInfo);
        }

    }
}
