using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.Models;
using MRA.Infrastructure.Excel;
using MRA.Infrastructure.Excel.Attributes;
using MRA.Infrastructure.Settings;
using MRA.Services.Excel.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;
using System.Reflection;

namespace MRA.Services.Excel;

public class ExcelService : IExcelService
{
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

    public const string EXCEL_FAVORITE_VALUE = "Favorite";
    public const string EXCEL_VISIBLE_VALUE = "Visible";
    public const string EXCEL_SEPARATOR_COMMENTS = "\n";

    public string FilePath { get { return _appConfiguration.EPPlus.File.Path; } }
    public string FileName { get { return _appConfiguration.EPPlus.File.Name; } }
    public string FileDateFormat { get { return _appConfiguration.EPPlus.File.DateFormat; } }
    public string FileExtension { get { return _appConfiguration.EPPlus.File.Extension; } }
    public bool SaveFileLocally { get { return FilePath != null && FileName != null; } }

    private readonly AppSettings _appConfiguration;
    private readonly ILogger<ExcelService>? _logger;
    private readonly IExcelProvider _excel;


    public ExcelService(
        AppSettings appConfig, 
        ILogger<ExcelService> logger, 
        IExcelProvider excelProvider)
    {
        _appConfiguration = appConfig;
        _logger = logger;
        _excel = excelProvider;
    }



    public void FillDrawingTable(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, List<DrawingModel> listDrawings)
    {
        _excel.SetTableHeaders(ref workSheet, properties);
        int numberDocuments = listDrawings.Count;
        int row = 2;
        foreach (var drawing in listDrawings)
        {
            _logger?.LogInformation($"Procesando \"{drawing.Id}\" ({row - 1}/{numberDocuments})");
            FillDrawingRow(ref workSheet, properties, drawing, row);
            row++;
        }

        _excel.CreateTable(ref workSheet, EXCEL_DRAWING_TABLE_NAME, 1, 1, listDrawings.Count + 1, properties.Count);
        _excel.SetBold(ref workSheet, 2, 1, listDrawings.Count + 1, 1);
    }

    public void FillDrawingRow(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, DrawingModel drawing, int row)
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
                        cell.Value = (valueBool ? EXCEL_VISIBLE_VALUE : "Oculto");
                        break;
                    case "Favorite":
                        cell.Value = (valueBool ? EXCEL_FAVORITE_VALUE : "");
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
                        cell.Value = String.Join(DrawingModel.SEPARATOR_TAGS, stringList);
                        break;
                    default:
                        cell.Value = String.Join(EXCEL_SEPARATOR_COMMENTS, stringList);
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
        _excel.CreateWorksheetDictionary<DrawingTypes>(
            excel,
            sheetName: EXCEL_STYLE_SHEET_NAME, 
            tableName: EXCEL_STYLE_TABLE_NAME,
            drawingProperties, workSheet,
            nameColumnDropdown: EXCEL_STYLE_COLUMN_NAME,
            nameColumnIndex: EXCEL_STYLE_COLUMN_INDEX);
        _excel.CreateWorksheetDictionary<DrawingProductTypes>(
            excel,
            sheetName: EXCEL_PRODUCT_SHEET_NAME,
            tableName: EXCEL_PRODUCT_TABLE_NAME,
            drawingProperties, workSheet,
            nameColumnDropdown: EXCEL_PRODUCT_COLUMN_NAME,
            nameColumnIndex: EXCEL_PRODUCT_COLUMN_INDEX);
        _excel.CreateWorksheetDictionary<DrawingSoftwares>(
            excel,
            sheetName: EXCEL_SOFTWARE_SHEET_NAME,
            tableName: EXCEL_SOFTWARE_TABLE_NAME,
            drawingProperties, workSheet,
            nameColumnDropdown: EXCEL_SOFTWARE_COLUMN_NAME,
            nameColumnIndex: EXCEL_SOFTWARE_COLUMN_INDEX);
        _excel.CreateWorksheetDictionary<DrawingPaperSizes>(
            excel,
            sheetName: EXCEL_PAPER_SHEET_NAME,
            tableName: EXCEL_PAPER_TABLE_NAME,
            drawingProperties, workSheet,
            nameColumnDropdown: EXCEL_PAPER_COLUMN_NAME,
            nameColumnIndex: EXCEL_PAPER_COLUMN_INDEX);
        _excel.CreateWorksheetDictionary<DrawingFilterTypes>(
            excel,
            sheetName: EXCEL_FILTER_SHEET_NAME,
            tableName: EXCEL_FILTER_TABLE_NAME,
            drawingProperties, workSheet,
            nameColumnDropdown: EXCEL_FILTER_COLUMN_NAME,
            nameColumnIndex: EXCEL_FILTER_COLUMN_INDEX);
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

    public DrawingModel ReadDrawingFromRow(ExcelWorksheet workSheet, List<ExcelColumnInfo> drawingProperties, Dictionary<string, int> nameToColumnMap, int row)
    {
        var drawing = new DrawingModel();

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

    private void SetPropertyValue(DrawingModel drawing, PropertyInfo property, object cellValue)
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
                        property.SetValue(drawing, cellValue.ToString() == EXCEL_VISIBLE_VALUE);
                        break;
                    case "Favorite":
                        property.SetValue(drawing, cellValue.ToString() == EXCEL_FAVORITE_VALUE);
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
                        list = cellValue.ToString().Split(new[] { DrawingModel.SEPARATOR_TAGS }, StringSplitOptions.None).ToList();
                        break;
                    default:
                        list = cellValue.ToString().Split(new[] { EXCEL_SEPARATOR_COMMENTS }, StringSplitOptions.None).ToList();
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
    public List<ExcelColumnInfo> GetPropertiesAttributes<T>()
    {
        return _excel.GetPropertiesAttributes<T>();
    }
}
