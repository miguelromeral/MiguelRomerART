using Microsoft.Extensions.Caching.Memory;
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


        /// <summary>
        /// Clear IMemoryCache
        /// </summary>
        /// <param name="cache">Cache</param>
        /// <exception cref="InvalidOperationException">Unable to clear memory cache</exception>
        /// <exception cref="ArgumentNullException">Cache is null</exception>
        public static void Clear(this IMemoryCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("Memory cache must not be null");
            }
            else if (cache is MemoryCache memCache)
            {
                memCache.Compact(1.0);
                return;
            }
            else
            {
                MethodInfo clearMethod = cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                if (clearMethod != null)
                {
                    clearMethod.Invoke(cache, null);
                    return;
                }
                else
                {
                    PropertyInfo prop = cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                    if (prop != null)
                    {
                        object innerCache = prop.GetValue(cache);
                        if (innerCache != null)
                        {
                            clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                            if (clearMethod != null)
                            {
                                clearMethod.Invoke(innerCache, null);
                                return;
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("Unable to clear memory cache instance of type " + cache.GetType().FullName);
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
