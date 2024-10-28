using MRA.DTO.Excel.Attributes;
using MRA.DTO.Firebase.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
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
        public static List<ExcelColumnInfo> GetPropertiesAttributes<T>()
        {
            return typeof(T)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ExcelColumnAttribute)))
                .Select(p => new ExcelColumnInfo()
                {
                    Property = p,
                    Attribute = (ExcelColumnAttribute)Attribute.GetCustomAttribute(p, typeof(ExcelColumnAttribute))
                })
                .OrderBy(p => p.Attribute.Order)
                .ToList();
        }

        public static void CreateTable(ref ExcelWorksheet workSheet, string name, int beginRow, int beginColumn, int endRow, int endColumn)
        {
            var dataRange = workSheet.Cells[beginRow, beginColumn, endRow, endColumn];
            var table = workSheet.Tables.Add(dataRange, name);
            table.TableStyle = TableStyles.Light6;
        }

        public static void SetBold(ref ExcelWorksheet workSheet, int beginRow, int beginColumn, int endRow, int endColumn)
        {
            using (var range = workSheet.Cells[beginRow, beginColumn, endRow, endColumn])
            {
                range.Style.Font.Bold = true;
            }
        }

        public static void StyleCellsHeader(ref ExcelWorksheet workSheet, int beginRow, int beginColumn, int endRow, int endColumn)
        {
            // Dar formato de color a la primera fila (encabezado)
            using (var range = workSheet.Cells[beginRow, beginColumn, endRow, endColumn])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.RebeccaPurple);
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.Font.Bold = true;
            }

            // Ajustar automáticamente el ancho de las columnas al contenido
            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
        }
    }
}
