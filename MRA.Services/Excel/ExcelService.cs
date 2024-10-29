using Microsoft.Extensions.Configuration;
using MRA.DTO.Excel.Attributes;
using MRA.DTO.Firebase.Models;
using MRA.Services.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Excel
{
    public class ExcelService
    {
        private readonly IConfiguration _configuration;
        private readonly ConsoleHelper? _console;

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

        public ExcelService(IConfiguration configuration, ConsoleHelper consoleHelper)
        {
            _configuration = configuration;
            _console = consoleHelper;
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
            int numberDocuments = listDrawings.Count;
            int row = 2;
            foreach (var drawing in listDrawings)
            {
                _console?.ShowMessageInfo($"Procesando documento ({row - 1}/{numberDocuments}): " + drawing.Id);
                FillTableRow(ref workSheet, properties, drawing, row);
                row++;
            }
        }

        public void FillTableRow(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, Drawing drawing, int row)
        {
            int col = 1;
            foreach (var prop in properties)
            {
                var cell = workSheet.Cells[row, col];
                object value = prop.Property.GetValue(drawing);
                cell.Value = value;


                if(value is bool valueBool)
                {
                    switch (prop.Attribute.Name)
                    {
                        case "Visible":
                            cell.Value = (valueBool ? Drawing.EXCEL_VISIBLE_VALUE : "Oculto");
                            break;
                        case "Favorite":
                            cell.Value = (valueBool ? Drawing.EXCEL_FAVORITE_VALUE : "");
                            break;
                        default:
                            cell.Value = (valueBool ? "TRUE" : "FALSE");
                            break;
                    }
                }
                else if (value is double || value is float || value is decimal)
                {
                    cell.Style.Numberformat.Format = "#,##0.00";
                }
                else if (value is int)
                {
                    cell.Style.Numberformat.Format = "#,##0";
                }
                else if (value is DateTime)
                {
                    cell.Style.Numberformat.Format = "yyyy/mm/dd";
                }
                else if (value is List<string> stringList)
                {
                    switch (prop.Attribute.Name)
                    {
                        case "Tags":
                            cell.Value = String.Join(Drawing.SEPARATOR_TAGS, stringList);
                            break;
                        default:
                            cell.Value = String.Join(Drawing.EXCEL_SEPARATOR_COMMENTS, stringList);
                            break;
                    }
                    cell.Style.WrapText = true;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    workSheet.Row(row).Height = 60;
                }
                if (prop.Attribute.URL && value is string valueUrl && !String.IsNullOrEmpty(valueUrl))
                {
                    cell.Hyperlink = new Uri(valueUrl);
                    cell.Style.Font.UnderLine = true;
                    cell.Style.Font.Color.SetColor(Color.Blue);
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

        public List<Drawing> ImportDrawingsFromExcel(FileInfo fileInfo)
        {
            var listDrawings = new List<Drawing>();

            using (var package = new ExcelPackage(fileInfo))
            {
                var workSheet = package.Workbook.Worksheets[SheetName];
                if (workSheet == null)
                {
                    throw new Exception($"Worksheet '{SheetName}' not found in the file.");
                }

                // Obtener propiedades de Drawing con atributos de Excel
                var drawingProperties = GetPropertiesAttributes<Drawing>();

                // Construir un mapeo entre los nombres de encabezado y la posición de columna
                var nameToColumnMap = new Dictionary<string, int>();
                for (int col = 1; col <= workSheet.Dimension.End.Column; col++)
                {
                    var headerValue = workSheet.Cells[1, col].Text;
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        nameToColumnMap[headerValue] = col;
                    }
                }

                // Leer cada fila (comenzando en la segunda fila, asumiendo encabezado en la primera)
                int row = 2;
                while (workSheet.Cells[row, 1].Value != null) // Suponiendo que siempre hay un valor en la primera columna
                {
                    var drawing = new Drawing();

                    foreach (var propInfo in drawingProperties.Where(x => x.Property.CanWrite))
                    {
                        if (!nameToColumnMap.TryGetValue(propInfo.Attribute.Name, out int col))
                        {
                            // Si el nombre de la columna no existe en el mapeo, continuar
                            continue;
                        }

                        var cell = workSheet.Cells[row, col];
                        var cellValue = cell.Value;

                        // Mapear el valor de la celda al tipo de la propiedad
                        SetPropertyValue(drawing, propInfo.Property, cellValue);
                    }

                    listDrawings.Add(drawing);
                    row++;
                }
            }

            return listDrawings;
        }


        private void SetPropertyValue(Drawing drawing, PropertyInfo property, object cellValue)
        {
            if (cellValue == null)
            {
                property.SetValue(drawing, null);
                return;
            }

            Type propType = property.PropertyType;

            try
            {
                if (propType == typeof(string))
                {
                    property.SetValue(drawing, cellValue.ToString());
                }
                else if (propType == typeof(int))
                {
                    property.SetValue(drawing, Convert.ToInt32(cellValue));
                }
                else if (propType == typeof(long))
                {
                    property.SetValue(drawing, Convert.ToInt64(cellValue));
                }
                else if (propType == typeof(double))
                {
                    property.SetValue(drawing, Convert.ToDouble(cellValue));
                }
                else if (propType == typeof(decimal))
                {
                    property.SetValue(drawing, Convert.ToDecimal(cellValue));
                }
                else if (propType == typeof(DateTime))
                {
                    property.SetValue(drawing, DateTime.FromOADate(Convert.ToDouble(cellValue)));
                }
                else if (propType == typeof(bool))
                {
                    switch (property.Name)
                    {
                        case "Visible":
                            property.SetValue(drawing, cellValue.ToString() == Drawing.EXCEL_VISIBLE_VALUE);
                            break;
                        case "Favorite":
                            property.SetValue(drawing, cellValue.ToString() == Drawing.EXCEL_FAVORITE_VALUE);
                            break;
                        default:
                            property.SetValue(drawing, cellValue.ToString() == "TRUE");
                            break;
                    }
                }
                else if (propType == typeof(List<string>))
                {
                    var list = new List<string>();
                    switch (property.Name)
                    {
                        case "Tags":
                            list = cellValue.ToString().Split(new[] { Drawing.SEPARATOR_TAGS }, StringSplitOptions.None).ToList();
                            break;
                        default:
                            list = cellValue.ToString().Split(new[] { Drawing.EXCEL_SEPARATOR_COMMENTS }, StringSplitOptions.None).ToList();
                            break;
                    }

                    property.SetValue(drawing, list);
                }
                else if (propType == typeof(Uri))
                {
                    if (Uri.IsWellFormedUriString(cellValue.ToString(), UriKind.Absolute))
                    {
                        property.SetValue(drawing, new Uri(cellValue.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting property {property.Name}: {ex.Message}");
            }
        }

    }
}
