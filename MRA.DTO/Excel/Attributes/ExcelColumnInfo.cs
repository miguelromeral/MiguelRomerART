using MRA.DTO.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Excel.Attributes
{
    public class ExcelColumnInfo
    {
        public PropertyInfo Property { get; set; }
        public ExcelColumnAttribute Attribute { get; set; }

        public override string ToString()
        {
            return $"{Attribute.Name} => {Property.PropertyType}";
        }
    }
}
