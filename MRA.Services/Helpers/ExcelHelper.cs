using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Helpers
{
    public class ExcelHelper
    {
        public static void StyleCellsHeader(ref ExcelWorksheet workSheet, int beginRow, int beginColumn, int endRow, int endColumn)
        {
            // Dar formato de color a la primera fila (encabezado)
            using (var range = workSheet.Cells[beginRow, beginColumn, endRow, endColumn])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Purple);
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.Font.Bold = true;
            }

            // Ajustar automáticamente el ancho de las columnas al contenido
            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
        }
    }
}
