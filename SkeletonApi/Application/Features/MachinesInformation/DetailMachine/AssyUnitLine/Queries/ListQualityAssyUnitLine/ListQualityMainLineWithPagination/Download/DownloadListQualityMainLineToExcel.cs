using ClosedXML.Excel;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLine;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLineWithPagination.Download
{
    public class DownloadListQualityMainLineToExcel
    {
        private readonly PaginatedResult<GetListQualityMainLineDto> pg;
        public DownloadListQualityMainLineToExcel(PaginatedResult<GetListQualityMainLineDto> getListQuality)
        {
            pg = getListQuality;
        }

        public void GetListExcel(ref byte[] _content, ref string FileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "date_time";
                worksheet.Cell(1, 2).Value = "frq_inverter";
                worksheet.Cell(1, 3).Value = "duration_stop";


                for (int i = 0; i < pg.Data.Count(); i++)
                {
                    worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                    worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).FrqInverter;
                    worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DurationStop;
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
