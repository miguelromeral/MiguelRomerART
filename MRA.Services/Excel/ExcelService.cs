using Microsoft.Extensions.Configuration;
using MRA.DTO.Excel.Attributes;
using MRA.DTO.Firebase.Models;
using MRA.DTO.Logger;
using MRA.Services.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Excel
{
    public class ExcelService
    {
        private const string APPSETTING_EPPLUS_LICENSE = "EPPlus:ExcelPackage:LicenseContext";
        private const string APPSETTING_EXCEL_FILE_PATH = "Excel:File:Path";
        private const string APPSETTING_EXCEL_FILE_NAME = "Excel:File:Name";
        private const string APPSETTING_EXCEL_FILE_DATE_FORMAT = "Excel:File:DateFormat";
        private const string APPSETTING_EXCEL_FILE_EXTENSION = "Excel:File:Extension";

        public const string EXCEL_DRAWING_SHEET_NAME = "Drawings";
        public const string EXCEL_DRAWING_TABLE_NAME = "TableDrawings";

        public const string EXCEL_STYLE_SHEET_NAME = "Styles";
        public const string EXCEL_STYLE_TABLE_NAME = "TableStyles";
        public const string EXCEL_STYLE_COLUMN_NAME = "Type";
        public const string EXCEL_STYLE_COLUMN_INDEX = "#Type";

        public const string EXCEL_PRODUCT_SHEET_NAME = "Products";
        public const string EXCEL_PRODUCT_TABLE_NAME = "TableProducts";
        public const string EXCEL_PRODUCT_COLUMN_NAME = "Product Type";
        public const string EXCEL_PRODUCT_COLUMN_INDEX = "#Product Type";

        public const string EXCEL_SOFTWARE_SHEET_NAME = "Software";
        public const string EXCEL_SOFTWARE_TABLE_NAME = "TableSoftware";
        public const string EXCEL_SOFTWARE_COLUMN_NAME = "Software";
        public const string EXCEL_SOFTWARE_COLUMN_INDEX = "#Software";

        public const string EXCEL_PAPER_SHEET_NAME = "Papers";
        public const string EXCEL_PAPER_TABLE_NAME = "TablePapers";
        public const string EXCEL_PAPER_COLUMN_NAME = "Paper";
        public const string EXCEL_PAPER_COLUMN_INDEX = "#Paper";

        public const string EXCEL_FILTER_SHEET_NAME = "Filters";
        public const string EXCEL_FILTER_TABLE_NAME = "TableFilters";
        public const string EXCEL_FILTER_COLUMN_NAME = "Filter";
        public const string EXCEL_FILTER_COLUMN_INDEX = "#Filter";

        private const string DICTIONARY_COLUMN_INDEX = "Index";
        private const string DICTIONARY_COLUMN_NAME = "Name";

        public string License { get { return _configuration[APPSETTING_EPPLUS_LICENSE]; } }
        public string FilePath { get { return _configuration[APPSETTING_EXCEL_FILE_PATH]; } }
        public string FileName { get { return _configuration[APPSETTING_EXCEL_FILE_NAME]; } }
        public string FileDateFormat { get { return _configuration[APPSETTING_EXCEL_FILE_DATE_FORMAT]; } }
        public string FileExtension { get { return _configuration[APPSETTING_EXCEL_FILE_EXTENSION]; } }
        public bool SaveFileLocally { get { return FilePath != null && FileName != null; } }

        private readonly IConfiguration _configuration;
        private readonly MRLogger? _logger;

        public ExcelService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ExcelService(IConfiguration configuration, MRLogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public List<ExcelColumnInfo> GetPropertiesAttributes<T>()
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

        public void FillDrawingTable(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, List<Drawing> listDrawings)
        {
            SetTableHeaders(ref workSheet, properties);
            int numberDocuments = listDrawings.Count;
            int row = 2;
            foreach (var drawing in listDrawings)
            {
                _logger?.Log($"Procesando \"{drawing.Id}\" ({row - 1}/{numberDocuments})");
                FillDrawingRow(ref workSheet, properties, drawing, row);
                row++;
            }

            CreateTable(ref workSheet, EXCEL_DRAWING_TABLE_NAME, 1, 1, listDrawings.Count + 1, properties.Count);
            SetBold(ref workSheet, 2, 1, listDrawings.Count + 1, 1);
        }

        public void FillDrawingRow(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, Drawing drawing, int row)
        {
            int col = 1;
            foreach (var prop in properties)
            {
                workSheet.Column(col).Width = prop.Attribute.Width;
                var cell = workSheet.Cells[row, col];
                object value = prop.GetValue(drawing);
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
                if (prop.Attribute.WrapText)
                {
                    cell.Style.WrapText = true;
                }
                if (prop.Attribute.Hidden && !workSheet.Column(col).Hidden)
                {
                    workSheet.Column(col).Hidden = true;
                }
                col++;
            }
        }



        public void FillSheetsDictionary(ExcelPackage excel, List<ExcelColumnInfo> drawingProperties, ExcelWorksheet workSheet)
        {
            CreateWorksheetDictionary(
                excel,
                sheetName: EXCEL_STYLE_SHEET_NAME, 
                tableName: EXCEL_STYLE_TABLE_NAME,
                Drawing.DRAWING_TYPES, drawingProperties, workSheet,
                nameColumnDropdown: EXCEL_STYLE_COLUMN_NAME,
                nameColumnIndex: EXCEL_STYLE_COLUMN_INDEX);
            CreateWorksheetDictionary(
                excel,
                sheetName: EXCEL_PRODUCT_SHEET_NAME,
                tableName: EXCEL_PRODUCT_TABLE_NAME,
                Drawing.DRAWING_PRODUCT_TYPES, drawingProperties, workSheet,
                nameColumnDropdown: EXCEL_PRODUCT_COLUMN_NAME,
                nameColumnIndex: EXCEL_PRODUCT_COLUMN_INDEX);
            CreateWorksheetDictionary(
                excel,
                sheetName: EXCEL_SOFTWARE_SHEET_NAME,
                tableName: EXCEL_SOFTWARE_TABLE_NAME, 
                Drawing.DRAWING_SOFTWARE, drawingProperties, workSheet,
                nameColumnDropdown: EXCEL_SOFTWARE_COLUMN_NAME,
                nameColumnIndex: EXCEL_SOFTWARE_COLUMN_INDEX);
            CreateWorksheetDictionary(
                excel,
                sheetName: EXCEL_PAPER_SHEET_NAME,
                tableName: EXCEL_PAPER_TABLE_NAME, 
                Drawing.DRAWING_PAPER_SIZE, drawingProperties, workSheet,
                nameColumnDropdown: EXCEL_PAPER_COLUMN_NAME,
                nameColumnIndex: EXCEL_PAPER_COLUMN_INDEX);
            CreateWorksheetDictionary(
                excel,
                sheetName: EXCEL_FILTER_SHEET_NAME,
                tableName: EXCEL_FILTER_TABLE_NAME, 
                Drawing.DRAWING_FILTER, drawingProperties, workSheet,
                nameColumnDropdown: EXCEL_FILTER_COLUMN_NAME,
                nameColumnIndex: EXCEL_FILTER_COLUMN_INDEX);
        }

        public static void CreateWorksheetDictionary(ExcelPackage excel, string sheetName, string tableName, Dictionary<int, string> dictionary, 
            List<ExcelColumnInfo> properties, ExcelWorksheet mainSheet, string nameColumnDropdown, string nameColumnIndex)
        {
            var wsTypes = FillWorksheetDictionary(excel, name: sheetName, tableName: tableName, dictionary);

            AddDropdownColumn(mainSheet, wsTypes, tableName, 
                dataRowStart: 2, 
                dropdownColumn: FindColumnNumberOf(properties, nameColumnDropdown), 
                indexColumn: FindColumnNumberOf(properties, nameColumnIndex));
        }

        public static int FindColumnNumberOf(List<ExcelColumnInfo> properties, string name)
        {
            var index = properties.FindIndex(x => x.Attribute.Name.Equals(name)) + 1;
            if (index < 0) throw new Exception($"Column with name \"{name}\" was not found");
            return index;
        }

        public static ExcelWorksheet FillWorksheetDictionary(ExcelPackage excel, string name, string tableName, Dictionary<int, string> dictionary)
        {
            var worksheet = excel.Workbook.Worksheets.Add(name);

            worksheet.Cells[1, 1].Value = DICTIONARY_COLUMN_NAME;
            worksheet.Cells[1, 2].Value = DICTIONARY_COLUMN_INDEX;

            int row = 1;
            foreach(var item in dictionary)
            {
                row++;
                worksheet.Cells[row, 1].Value = item.Value;
                worksheet.Cells[row, 2].Value = item.Key;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            }

            CreateTable(ref worksheet, tableName, 1, 1, row, 2);
            worksheet.Column(1).Width = 60;
            return worksheet;
        }

        public static void AddDropdownColumn(ExcelWorksheet mainSheet, ExcelWorksheet dictionarySheet, string tableName, int dataRowStart, int dropdownColumn, int indexColumn)
        {
            // Obtener la tabla desde el worksheet del diccionario
            var dictionaryTable = dictionarySheet.Tables[tableName];
            if (dictionaryTable == null)
                throw new Exception($"La tabla '{tableName}' no fue encontrada en la hoja '{dictionarySheet.Name}'.");

            // Definir el nombre de rango dinámico para la columna "Name" en la tabla del diccionario
            string dynamicRangeName = $"{tableName}_NameRange";

            // Obtener la columna de "Name" en la tabla de diccionario
            int nameColumnIndex = dictionaryTable.Columns[DICTIONARY_COLUMN_NAME].Position + dictionaryTable.Address.Start.Column;

            // Crear el rango dinámico usando un rango estructurado
            dictionarySheet.Workbook.Names.Add(dynamicRangeName, dictionarySheet.Cells[dictionaryTable.Address.Start.Row + 1, nameColumnIndex, dictionaryTable.Address.End.Row, nameColumnIndex]);

            // Agregar validación de lista en cada celda de la columna de dropdown en la hoja principal usando el nombre del rango
            for (int row = dataRowStart; row <= mainSheet.Dimension.End.Row; row++)
            {
                var validation = mainSheet.DataValidations.AddListValidation(mainSheet.Cells[row, dropdownColumn].Address);
                validation.Formula.ExcelFormula = dynamicRangeName;

                // Obtener la dirección de la celda del dropdown para referencia en VLOOKUP
                string dropdownCellAddress = mainSheet.Cells[row, dropdownColumn].Address;

                // Asignar la fórmula VLOOKUP en cada celda de indexColumn
                mainSheet.Cells[row, indexColumn].Formula = $"VLOOKUP({dropdownCellAddress}, '{dictionarySheet.Name}'!A:B, 2, FALSE)";
            }
        }


        public static void CreateTable(ref ExcelWorksheet workSheet, string name, int beginRow, int beginColumn, int endRow, int endColumn)
        {
            var dataRange = workSheet.Cells[beginRow, beginColumn, endRow, endColumn];
            var table = workSheet.Tables.Add(dataRange, name);
            table.TableStyle = TableStyles.Light6;
            ExcelService.StyleCellsHeader(ref workSheet, 1, 1, 1, endColumn);
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
            using (var range = workSheet.Cells[beginRow, beginColumn, endRow, endColumn])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.RebeccaPurple);
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.Font.Bold = true;
            }

            //workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
        }

        public string GetFileName() => $"{FileName}" + $"_{DateTime.Now.ToString(FileDateFormat)}" + $".{FileExtension}";

        public FileInfo GetFileInfo()
        {
            var fileName = GetFileName();
            var filePath = Path.Combine(FilePath, fileName);

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            return new FileInfo(filePath);
        }

        public Dictionary<string, int> GetColumnMapDrawing(ExcelWorksheet workSheet)
        {
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

            return nameToColumnMap;
        }

        public Drawing ReadDrawingFromRow(ExcelWorksheet workSheet, List<ExcelColumnInfo> drawingProperties, Dictionary<string, int> nameToColumnMap, int row)
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

            return drawing;
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
