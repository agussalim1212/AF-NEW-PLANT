using ClosedXML.Excel;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityNutRunnerSS_RWWithPagination.Download
{
    public class DownloadListQualityNutRunnerToExcel
    {
        private readonly PaginatedResult<GetListQualityNutRunnerSteeringStemDto> pg;
        public DownloadListQualityNutRunnerToExcel(PaginatedResult<GetListQualityNutRunnerSteeringStemDto> getListQuality)
        {
            pg = getListQuality;
        }

        public void GetListExcel(ref byte[] _content, ref string FileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "date_time";
                worksheet.Cell(1, 2).Value = "status";
                worksheet.Cell(1, 3).Value = "data_barcode";
                worksheet.Cell(1, 3).Value = "data_torsi";

                for (int i = 0; i < pg.Data.Count(); i++)
                {
                    worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                    worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                    worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataBarcode;
                    worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataTorQ;

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
