using MRA.DTO.Models;
using MRA.Infrastructure.Excel.Attributes;
using OfficeOpenXml;

namespace MRA.Services.Excel.Interfaces;

public interface IExcelService
{
    string GetFileName();

    void FillDrawingTable(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, List<DrawingModel> listDrawings);
    void FillSheetsDictionary(ExcelPackage excel, List<ExcelColumnInfo> properties, ExcelWorksheet workSheet);
    
    List<ExcelColumnInfo> GetPropertiesAttributes<T>();
}
