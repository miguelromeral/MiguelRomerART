
using MRA.DTO.Excel.Attributes;
using MRA.DTO.Models;
using OfficeOpenXml;

namespace MRA.Services.Excel.Interfaces;

public interface IExcelService
{
    string GetEPPlusLicense();
    string GetFileName();

    List<ExcelColumnInfo> GetPropertiesAttributes<T>();

    void FillDrawingTable(ref ExcelWorksheet workSheet, List<ExcelColumnInfo> properties, List<DrawingModel> listDrawings);
    void FillSheetsDictionary(ExcelPackage excel, List<ExcelColumnInfo> properties, ExcelWorksheet workSheet);
}
