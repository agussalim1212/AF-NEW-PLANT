using ClosedXML.Excel;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Queries.DownloadList
{
    public class DownloadListMaintCorrectiveToExcel
    {
        private readonly PaginatedResult<DownloadListMaintCorrectiveDto> pg;

        public DownloadListMaintCorrectiveToExcel(PaginatedResult<DownloadListMaintCorrectiveDto> paginated)
        {
            pg = paginated;
        }

        public void GetListExcel(ref byte[] _content, ref string FileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                worksheet.Cell(1, 1).Value = "machine_name";
                worksheet.Cell(1, 2).Value = "start_date";
                worksheet.Cell(1, 3).Value = "actual";
                worksheet.Cell(1, 4).Value = "end_date";

                for (int i = 0; i < pg.Data.Count(); i++)
                {
                    worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).Name;
                    worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).StartDate;
                    worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).Actual;
                    worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).EndDate;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    _content = stream.ToArray();
                    FileName = $"List_Maintenance_Corrective_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx";
                }
            }
        }
    }
}
