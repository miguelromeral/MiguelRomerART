using Microsoft.Extensions.Configuration;
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

namespace MRA.Services.Excel
{
    public class ExcelService
    {
        private IConfiguration _configuration;

        private const string APPSETTING_EPPLUS_LICENSE = "EPPlus:ExcelPackage:LicenseContext";
        private const string APPSETTING_EXCEL_FILE_PATH = "Excel:File:Path";
        private const string APPSETTING_EXCEL_FILE_NAME = "Excel:File:Name";
        private const string APPSETTING_EXCEL_FILE_DATE_FORMAT = "Excel:File:DateFormat";
        private const string APPSETTING_EXCEL_FILE_EXTENSION = "Excel:File:Extension";

        private const string APPSETTING_EXCEL_SHEET_NAME = "Excel:Sheet:Name";
        private const string APPSETTING_EXCEL_TABLE_NAME = "Excel:Table:Name";

        public string License { get { return _configuration[APPSETTING_EPPLUS_LICENSE]; } }
        public string FilePath { get { return _configuration[APPSETTING_EXCEL_FILE_PATH]; } }
        public string FileName { get { return _configuration[APPSETTING_EXCEL_FILE_NAME]; } }
        public string FileDateFormat { get { return _configuration[APPSETTING_EXCEL_FILE_DATE_FORMAT]; } }
        public string FileExtension { get { return _configuration[APPSETTING_EXCEL_FILE_EXTENSION]; } }
        public string SheetName { get { return _configuration[APPSETTING_EXCEL_SHEET_NAME]; } }
        public string TableName { get { return _configuration[APPSETTING_EXCEL_TABLE_NAME]; } }



        public ExcelService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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

        public void SetTableHeaders(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties)
        {
            int col = 1;
            foreach (var prop in properties)
            {
                workSheet.Cells[1, col].Value = prop.Attribute.Name;
                col++;
            }
        }

        public void FillTable(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, List<Drawing> listDrawings)
        {
            //int numberDocuments = listDrawings.Count;
            int row = 2;
            foreach (var drawing in listDrawings)
            {
                FillTableRow(ref workSheet, properties, drawing, row);
                row++;
            }
        }

        public void FillTableRow(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, Drawing drawing, int row)
        {
            //helper.ShowMessageInfo($"Procesando documento ({row - 1}/{numberDocuments}): " + drawing.Id);
            int col = 1;
            foreach (var prop in properties)
            {
                var cell = workSheet.Cells[row, col];
                cell.Value = prop.Property.GetValue(drawing);
                if (prop.Attribute.WrapText)
                {
                    cell.Style.WrapText = true;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Row(row).Height = 60;
                    //workSheet.Column(col).Width = 200;
                }
                if (prop.Attribute.Hidden && !workSheet.Column(col).Hidden)
                {
                    workSheet.Column(col).Hidden = true;
                }
                col++;
            }
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

        public FileInfo GetFileInfo()
        {
            var fileName = $"{FileName}" + $"_{DateTime.Now.ToString(FileDateFormat)}" + $".{FileExtension}";
            var filePath = Path.Combine(FilePath, fileName);

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            return new FileInfo(filePath);
        }
    }
}
