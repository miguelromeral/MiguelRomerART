using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Firebase.Models
{
    public class Resume
    {
        public List<Experience> Experiences { get; set; }
        public List<Certification> Certifications { get; set; }

    }

    public class Certification
    {
        public string IssuedBy { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string BadgeSrc { get; set; }
        public DateTime Date { get; set; }
    }
}
