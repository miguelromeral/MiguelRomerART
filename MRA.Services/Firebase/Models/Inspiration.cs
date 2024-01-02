using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Models
{
    public class Inspiration
    {

        public static Dictionary<int, string> INSPIRATION_TYPES = new Dictionary<int, string>()
            {
                {1, "Profesionales"},
                {2, "Aficionados"},
                {3, "Modelos"},
            };
        public string Id { get; set; }
        public string Name { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public int Type { get; set; }
        public string TypeName
        {
            get
            {
                try
                {
                    return INSPIRATION_TYPES[Type];
                }
                catch (Exception ex)
                {
                    return "Otros";
                }
            }
        }
    }
}
