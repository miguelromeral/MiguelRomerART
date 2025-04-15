using System.Globalization;
using System.Reflection;

namespace MRA.DTO
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

        public static string GetStringFromDate(DateTime date)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("es-ES");
            return date.ToString("yyyy/MM/dd", cultureInfo);
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

        public static string FormattedDateInput(string Date)
        {
            if (String.IsNullOrEmpty(Date))
                return "";

            DateTime date = DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var cultureInfo = CultureInfo.GetCultureInfo("es-ES");
            return date.ToString("yyyy-MM-dd");

        }

        public static double CalculatePopularity(double valor, double puntuacionMaxima, double min = 0, double max = 100)
        {
            if (valor < min || valor > max)
            {
                return 0;
            }

            double puntuacion = (valor - min) * puntuacionMaxima / (max - min);
            return puntuacion;
        }

        public static double CalculatePopularity(DateTime fecha, double puntuacionMaxima, DateTime fechaMin, DateTime fechaMax)
        {
            if (fecha < fechaMin || fecha > fechaMax)
            {
                return 0;
            }

            long ticksFecha = fecha.Ticks;
            long ticksMin = fechaMin.Ticks;
            long ticksMax = fechaMax.Ticks;

            return CalculatePopularity(ticksFecha, puntuacionMaxima, ticksMin, ticksMax);
        }
        public static DateTime ConvertirStringADateTime(string fechaString, string formato = "yyyy/MM/dd")
        {
            if (String.IsNullOrEmpty(fechaString))
            {
                return DateTime.MinValue;
            }

            DateTime fecha = DateTime.ParseExact(fechaString, formato, CultureInfo.InvariantCulture);
            return fecha;
        }
    }

}
