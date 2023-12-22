using ClosedXML.Excel;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFiling;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFilingWithPagination.Download
{
    public class DownloadListQualityCoolantFilingToExcel
    {
        private readonly PaginatedResult<GetListQualityCoolantFilingDto> pg;
        public DownloadListQualityCoolantFilingToExcel(PaginatedResult<GetListQualityCoolantFilingDto> getListQuality)
        {
            pg = getListQuality;
        }

        public void GetListExcel(ref byte[] _content, ref string FileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "date_time";
                worksheet.Cell(1, 2).Value = "volume_coolant";
                worksheet.Cell(1, 3).Value = "data_barcode";


                for (int i = 0; i < pg.Data.Count(); i++)
                {
                    worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                    worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).VolumeCoolant;
                    worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataBarcode;

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    _content = stream.ToArray();
                    FileName = $"List_Quality_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx";
                }
            }

        }
    }
}
