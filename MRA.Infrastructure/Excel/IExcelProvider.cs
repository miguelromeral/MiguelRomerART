﻿using MRA.Infrastructure.Excel.Attributes;
using OfficeOpenXml;

namespace MRA.Infrastructure.Excel;

public interface IExcelProvider
{

    List<ExcelColumnInfo> GetPropertiesAttributes<T>();

    void SetTableHeaders(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties);
    void CreateWorksheetDictionary(ExcelPackage excel, string sheetName, string tableName, Dictionary<int, string> dictionary,
        List<ExcelColumnInfo> properties, ExcelWorksheet mainSheet, string nameColumnDropdown, string nameColumnIndex);
    void CreateWorksheetDictionary<TEnum>(ExcelPackage excel, string sheetName, string tableName,
        List<ExcelColumnInfo> properties, ExcelWorksheet mainSheet, string nameColumnDropdown, string nameColumnIndex) where TEnum : Enum;
    
    void StyleCellsHeader(ref ExcelWorksheet workSheet, int beginRow, int beginColumn, int endRow, int endColumn);
    void SetBold(ref ExcelWorksheet workSheet, int beginRow, int beginColumn, int endRow, int endColumn);
    void CreateTable(ref ExcelWorksheet workSheet, string name, int beginRow, int beginColumn, int endRow, int endColumn);
    Dictionary<string, int> GetColumnMapDrawing(ExcelWorksheet workSheet);
    ExcelWorksheet FillWorksheetDictionary(ExcelPackage excel, string name, string tableName, Dictionary<int, string> dictionary);
    ExcelWorksheet FillWorksheetDictionary<TEnum>(ExcelPackage excel, string name, string tableName) where TEnum : Enum;
    int FindColumnNumberOf(List<ExcelColumnInfo> properties, string name);
}
