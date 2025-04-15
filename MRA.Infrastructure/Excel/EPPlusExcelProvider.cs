using Microsoft.Extensions.Logging;
using MRA.Infrastructure.Settings;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using MRA.Infrastructure.Excel.Attributes;
using System.Drawing;

namespace MRA.Infrastructure.Excel;

public class EPPlusExcelProvider : IExcelProvider
{
    private const string DICTIONARY_COLUMN_INDEX = "Index";
    private const string DICTIONARY_COLUMN_NAME = "Name";

    private readonly AppSettings _appConfiguration;
    private readonly ILogger<EPPlusExcelProvider>? _logger;


    public EPPlusExcelProvider(AppSettings appConfig, ILogger<EPPlusExcelProvider> logger)
    {
        _appConfiguration = appConfig;
        _logger = logger;
        ExcelPackage.License.SetNonCommercialPersonal("MiguelRomeral");
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

    public void CreateWorksheetDictionary(ExcelPackage excel, string sheetName, string tableName, Dictionary<int, string> dictionary,
        List<ExcelColumnInfo> properties, ExcelWorksheet mainSheet, string nameColumnDropdown, string nameColumnIndex)
    {
        var wsTypes = FillWorksheetDictionary(excel, name: sheetName, tableName: tableName, dictionary);

        AddDropdownColumn(mainSheet, wsTypes, tableName,
            dataRowStart: 2,
            dropdownColumn: FindColumnNumberOf(properties, nameColumnDropdown),
            indexColumn: FindColumnNumberOf(properties, nameColumnIndex));
    }

    public int FindColumnNumberOf(List<ExcelColumnInfo> properties, string name)
    {
        var index = properties.FindIndex(x => x.Attribute.Name.Equals(name)) + 1;
        if (index < 0) throw new Exception($"Column with name \"{name}\" was not found");
        return index;
    }

    public ExcelWorksheet FillWorksheetDictionary(ExcelPackage excel, string name, string tableName, Dictionary<int, string> dictionary)
    {
        var worksheet = excel.Workbook.Worksheets.Add(name);

        worksheet.Cells[1, 1].Value = DICTIONARY_COLUMN_NAME;
        worksheet.Cells[1, 2].Value = DICTIONARY_COLUMN_INDEX;

        int row = 1;
        foreach (var item in dictionary)
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

    private void AddDropdownColumn(ExcelWorksheet mainSheet, ExcelWorksheet dictionarySheet, string tableName, int dataRowStart, int dropdownColumn, int indexColumn)
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


    public void CreateTable(ref ExcelWorksheet workSheet, string name, int beginRow, int beginColumn, int endRow, int endColumn)
    {
        var dataRange = workSheet.Cells[beginRow, beginColumn, endRow, endColumn];
        var table = workSheet.Tables.Add(dataRange, name);
        table.TableStyle = TableStyles.Light6;
        StyleCellsHeader(ref workSheet, 1, 1, 1, endColumn);
    }

    public void SetBold(ref ExcelWorksheet workSheet, int beginRow, int beginColumn, int endRow, int endColumn)
    {
        using (var range = workSheet.Cells[beginRow, beginColumn, endRow, endColumn])
        {
            range.Style.Font.Bold = true;
        }
    }

    public void StyleCellsHeader(ref ExcelWorksheet workSheet, int beginRow, int beginColumn, int endRow, int endColumn)
    {
        using (var range = workSheet.Cells[beginRow, beginColumn, endRow, endColumn])
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.RebeccaPurple);
            range.Style.Font.Color.SetColor(Color.White);
            range.Style.Font.Bold = true;
        }
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
}
