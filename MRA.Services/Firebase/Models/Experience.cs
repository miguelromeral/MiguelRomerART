using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Models
{
    public class Experience
    {
        public Experience()
        {
            Aptitudes = new List<string>();
            Tecnologias = new List<string>();
            ProyectosSubcontratados = new List<Experience>();
        }

        public string Id { get; set; }
        public string Empresa { get; set; }
        public string Address { get; set; }
        public string Link { get; set; }
        public string LogoSrc { get; set; }
        public string Descripcion { get; set; }
        public string BannerColor { get; set; }
        public string Periodo { get; set; }
        public int Orden { get; set; }
        public List<string> Aptitudes { get; set; }
        public List<string> Tecnologias { get; set; }
        public List<Experience> ProyectosSubcontratados { get; set; }
    }

    //public class ExperienceSubcontratado : Experience
    //{
    //    public string IdSubcontratado { get; set; }
    //}
}
