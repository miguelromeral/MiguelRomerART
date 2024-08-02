using MRA.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.ViewModels.Art
{
    public class SaveDrawingRequest : Drawing
    {
        public bool IsEditing { get; set; }
    }
}
