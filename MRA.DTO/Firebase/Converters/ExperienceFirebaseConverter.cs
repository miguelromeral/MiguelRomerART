using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Interfaces;
using MRA.DTO.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.Converters
{
    public class ExperienceFirebaseConverter : IFirebaseConverter<Experience, ExperienceDocument>
    {
        public Experience ConvertToModel(ExperienceDocument document)
        {
            return new Experience
            {
                Id = document.Id,
                Empresa = document.empresa,
                BannerColor = document.banner_color
            };
        }

        public ExperienceDocument ConvertToDocument(Experience model)
        {
            return new ExperienceDocument
            {
                Id = model.Id,
                empresa = model.Empresa,
                banner_color = model.BannerColor
            };
        }
    }
}
