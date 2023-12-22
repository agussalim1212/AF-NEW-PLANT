using ClosedXML.Excel;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination.Download
{
    public class DownloadListQualityWheelFrontToExcel
    {
        private readonly PaginatedResult<GetListWheelFrontDto> pg;

        public DownloadListQualityWheelFrontToExcel(PaginatedResult<GetListWheelFrontDto> getListQuality)
        {
            pg = getListQuality;
        }
        public void GetListExcel(ref byte[] _content, ref string type_wheel, ref string FileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                if (type_wheel == "final_inspection")
                {

                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "status";
                    worksheet.Cell(1, 3).Value = "data_dial_horizontal";
                    worksheet.Cell(1, 4).Value = "data_dial_vertical";

                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                        worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataDialHorizontal;
                        worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataDialVertical;
                    }
                }
                else if (type_wheel == "tire_inflation")
                {
                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "tire_inflation";


                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).TirePresure;
                    }
                }
                else if (type_wheel == "disk_brake")
                {
                    worksheet.Cell(1, 1).Value = "date_time";
                    worksheet.Cell(1, 2).Value = "data_torQ";

                    for (int i = 0; i < pg.Data.Count(); i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                        worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).DataTorQ;
                    }
                }
                else
                {
                    type_wheel = "press_cone_race";
                    {
                        worksheet.Cell(1, 1).Value = "date_time";
                        worksheet.Cell(1, 2).Value = "status";
                        worksheet.Cell(1, 3).Value = "data_distance";
                        worksheet.Cell(1, 4).Value = "data_tonase";


                        for (int i = 0; i < pg.Data.Count(); i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = pg.Data.ElementAt(i).DateTime;
                            worksheet.Cell(i + 2, 2).Value = pg.Data.ElementAt(i).Status;
                            worksheet.Cell(i + 2, 3).Value = pg.Data.ElementAt(i).DataDistance;
                            worksheet.Cell(i + 2, 4).Value = pg.Data.ElementAt(i).DataTonase;

                        }
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    _content = stream.ToArray();
                    FileName = $"List_Quality_{type_wheel}_{DateTime.Now.ToString("yyyy-MMMM-dddd")}.xlsx";

                }
            }
        }
    }
}
