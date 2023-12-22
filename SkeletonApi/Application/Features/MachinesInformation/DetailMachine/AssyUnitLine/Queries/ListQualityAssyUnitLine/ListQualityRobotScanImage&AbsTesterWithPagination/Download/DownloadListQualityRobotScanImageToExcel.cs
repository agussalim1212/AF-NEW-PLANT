using ClosedXML.Excel;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage_AbsTesterWithPagination.Download
{
    public class DownloadListQualityRobotScanImageToExcel
    {
        private readonly PaginatedResult<GetListQualityRobotScanImageDto> pg;
        public DownloadListQualityRobotScanImageToExcel(PaginatedResult<GetListQualityRobotScanImageDto> getListQuality)
        {
            pg = getListQuality;
        }

        public void GetListExcel(ref byte[] _content, ref string FileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "date_time";
                worksheet.Cell(1, 2).Value = "data_barcode";
                worksheet.Cell(1, 3).Value = "status";

                for (int i = 0; i < pg.Data.Count(); i++)
                {
                    worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                    worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).DataBarcode;
                    worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).Status;

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
