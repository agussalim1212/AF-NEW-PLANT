using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.DTOs.Consumption;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityRobotScanImage;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityCoolantFilingWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityMainLineWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrakeWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityPressConeRaceWithPagination;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Configuration;
using SkeletonApi.Persistence.Contexts;

namespace SkeletonApi.Persistence.Repositories
{
    public class DetailAssyUnitRepository : IDetailAssyUnitRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly ApplicationDbContext _dbContext;

        public DetailAssyUnitRepository(IDapperReadDbConnection dapperReadDbConnection, ApplicationDbContext dbContext)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _dbContext = dbContext;
        }

        public async Task<List<GetListQualityCoolantFilingDto>> GetAllListQualityCoolantFiling(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityCoolantFilingDto> dt = new List<GetListQualityCoolantFilingDto>();
            var data = new GetListQualityCoolantFilingDto();

            var volumeVid = machine.Where(m => m.Subject.Vid.Contains("VOL-COLN")).FirstOrDefault();
            var barcodeVid = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();

            switch (type)
            {
                case "day":

                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",
                            };
                        }
                        else
                        {
                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);

                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                case "week":

                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",
                            };
                        }
                        else
                        {
                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);

                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                case "month":

                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",
                            };
                        }
                        else
                        {
                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);

                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                case "year":

                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = volumeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = ANY(@vid)
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",
                            };
                        }
                        else
                        {
                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                default:

                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var volumeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                        new { vid = volumeVid.Subject.Vid, now = DateTime.Now.Date });

                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<CoolantFilingConsumption>
                        (@"SELECT * FROM ""list_quality_coolant_filing"" WHERE id = @vid
                            AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                            ORDER BY  bucket DESC",
                        new { vid = barcodeVid.Subject.Vid, now = DateTime.Now.Date });

                        if (volumeConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityCoolantFilingDto
                            {
                                DateTime = DateTime.Now,
                                VolumeCoolant = 0,
                                DataBarcode = "-",
                            };
                        }
                        else
                        {
                            foreach (var f in barcodeConsumption)
                            {
                                GetListQualityCoolantFilingDto listQuality = new GetListQualityCoolantFilingDto();

                                var vol = volumeConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (vol != null)
                                {
                                    listQuality.VolumeCoolant = Convert.ToDecimal(vol.Value);
                                }
                                var barcode = barcodeConsumption.Where(k => k.Bucket == vol.Bucket).FirstOrDefault();
                                if (barcode != null)
                                {
                                    listQuality.DataBarcode = barcode.Value;
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;
            }
            return dt;
        }

        public async Task<List<GetListQualityMainLineDto>> GetAllListQualityMainLine(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityMainLineDto> dt = new List<GetListQualityMainLineDto>();
            var data = new GetListQualityMainLineDto();
            var frqVid = machine.Where(m => m.Subject.Vid.Contains("FREQUENCY")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (frqConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = null,
                                FrqInverter = null
                            };
                        }
                        else
                        {
                            foreach (var f in frqConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                listQuality.FrqInverter = Convert.ToDecimal(f.Value);
                                listQuality.DateTime = f.Bucket.AddHours(7).ToString("dd-MM-yyyy hh:mm:ss");
                                dt.Add(listQuality);
                            }
                        }
                    }

                    break;

                case "week":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (frqConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = null,
                                FrqInverter = null
                            };
                        }
                        else
                        {
                            foreach (var f in frqConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                listQuality.FrqInverter = Convert.ToDecimal(f.Value);
                                listQuality.DateTime = f.Bucket.AddHours(7).ToString("dd-MM-yyyy hh:mm:ss");
                                dt.Add(listQuality);
                            }
                        }
                    }

                    break;

                case "month":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (frqConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = null,
                                FrqInverter = null
                            };
                        }
                        else
                        {
                            foreach (var f in frqConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                listQuality.FrqInverter = Convert.ToDecimal(f.Value);
                                listQuality.DateTime = f.Bucket.AddHours(7).ToString("dd-MM-yyyy hh:mm:ss");
                                dt.Add(listQuality);
                            }
                        }
                    }

                    break;

                case "year":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = frqVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (frqConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = null,
                                FrqInverter = null
                            };
                        }
                        else
                        {
                            foreach (var f in frqConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                listQuality.FrqInverter = Convert.ToDecimal(f.Value);
                                listQuality.DateTime = f.Bucket.AddHours(7).ToString("dd-MM-yyyy hh:mm:ss");
                                dt.Add(listQuality);
                            }
                        }
                    }

                    break;

                default:
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        DateTime today = DateTime.Now.Date;
                        var frqConsumption = await _dapperReadDbConnection.QueryAsync<MainLineConsumption>
                        (@"SELECT * FROM ""list_quality_main_line"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @today)
                        ORDER BY bucket DESC",
                        new { vid = frqVid.Subject.Vid, today });

                        if (frqConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityMainLineDto
                            {
                                DateTime = null,
                                FrqInverter = null
                            };
                        }
                        else
                        {
                            foreach (var f in frqConsumption)
                            {
                                GetListQualityMainLineDto listQuality = new GetListQualityMainLineDto();

                                listQuality.FrqInverter = Convert.ToDecimal(f.Value);
                                listQuality.DateTime = f.Bucket.AddHours(7).ToString("dd-MM-yyyy hh:mm:ss");
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;
            }
            return dt;
        }

        public async Task<List<GetListQualityNutRunnerSteeringStemDto>> GetAllListQualityNutRunnerStem(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityNutRunnerSteeringStemDto> dt = new List<GetListQualityNutRunnerSteeringStemDto>();
            var data = new GetListQualityNutRunnerSteeringStemDto();

            var bcVid = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
            var statusVid = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();
            var torsiVid = machine.Where(m => m.Subject.Vid.Contains("TORQ")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {
                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                case "week":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('week', bucket) >= date_trunc('week', @starttime::date)
                        AND date_trunc('week', bucket) <= date_trunc('week', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {
                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                case "month":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('month', bucket) >= date_trunc('month', @starttime::date)
                        AND date_trunc('month', bucket) <= date_trunc('month', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {
                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                case "year":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = bcVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = statusVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('year', bucket) >= date_trunc('year', @starttime::date)
                        AND date_trunc('year', bucket) <= date_trunc('year', @endtime::date)
                        ORDER BY id DESC, bucket DESC", new { vid = torsiVid.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {
                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                default:
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                         AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                         ORDER BY  bucket DESC", new { vid = bcVid.Subject.Vid, now = DateTime.Now.Date });

                        var torsiConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC", new { vid = torsiVid.Subject.Vid, now = DateTime.Now.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<Consumption>
                        (@"SELECT * FROM ""list_quality_nut_runner_steering_stem_and_rear_wheel"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC", new { vid = statusVid.Subject.Vid, now = DateTime.Now.Date, });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityNutRunnerSteeringStemDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                                DataTorQ = 0
                            };
                        }
                        else
                        {
                            foreach (var f in statusConsumption)
                            {
                                GetListQualityNutRunnerSteeringStemDto listQuality = new GetListQualityNutRunnerSteeringStemDto();

                                var TorQ = torsiConsumption.Where(o => o.Bucket == f.Bucket).FirstOrDefault();
                                if (TorQ != null)
                                {
                                    listQuality.DataTorQ = Convert.ToDecimal(TorQ.Value);
                                }
                                var Barcode = barcodeConsumption.Where(k => k.Bucket == TorQ.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == f.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = f.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;
            }
            return dt;
        }

        public async Task<List<GetListQualityOilBrakeDto>> GetAllListQualityOilBrake(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityOilBrakeDto> dt = new List<GetListQualityOilBrakeDto>();
            var data = new GetListQualityOilBrakeDto();

            var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
            var leak = machine.Where(m => m.Subject.Vid.Contains("LEAK-TES")).FirstOrDefault();
            var vol = machine.Where(m => m.Subject.Vid.Contains("VOL-OIL-BRAEK")).FirstOrDefault();
            var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();
            var code = machine.Where(m => m.Subject.Vid.Contains("CODE")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = bc.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var leakTesterConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = leak.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var volumeOilConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = vol.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = status.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var codeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY  bucket DESC",
                        new { vid = code.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityOilBrakeDto
                            {
                                DateTime = DateTime.Now,
                                DataBarcode = "-",
                                LeakTester = 0,
                                VolumeOilBrake = 0,
                                Status = "-",
                                ErrorCode = 0
                            };
                        }
                        else
                        {
                            foreach (var s in statusConsumption)
                            {
                                GetListQualityOilBrakeDto listQuality = new GetListQualityOilBrakeDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }
                                var leakTester = leakTesterConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (leakTester != null)
                                {
                                    listQuality.LeakTester = Convert.ToDecimal(leakTester.Value);
                                }
                                var volumeOil = volumeOilConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (volumeOil != null)
                                {
                                    listQuality.VolumeOilBrake = Convert.ToDecimal(volumeOil.Value);
                                }
                                var errorCode = codeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (errorCode != null)
                                {
                                    listQuality.ErrorCode = Convert.ToInt32(errorCode.Value);
                                }
                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                default:
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                    (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                    ORDER BY  bucket DESC",
                    new { vid = bc.Subject.Vid, now = DateTime.Now.Date });

                        var leakTesterConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                    ORDER BY  bucket DESC",
                        new { vid = leak.Subject.Vid, now = DateTime.Now.Date });

                        var volumeOilConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                    ORDER BY  bucket DESC",
                        new { vid = vol.Subject.Vid, now = DateTime.Now.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                    ORDER BY  bucket DESC",
                        new { vid = status.Subject.Vid, dateNow = DateTime.Now.Date, });

                        var codeConsumption = await _dapperReadDbConnection.QueryAsync<OilBrakeConsumption>
                        (@"SELECT * FROM ""list_quality_oil_brake"" WHERE id = @vid
                    AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                    ORDER BY  bucket DESC",
                        new { vid = code.Subject.Vid, dateNow = DateTime.Now.Date, });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityOilBrakeDto
                            {
                                DateTime = DateTime.Now,
                                DataBarcode = "-",
                                LeakTester = 0,
                                VolumeOilBrake = 0,
                                Status = "-",
                                ErrorCode = 0
                            };
                        }
                        else
                        {
                            foreach (var s in statusConsumption)
                            {
                                GetListQualityOilBrakeDto listQuality = new GetListQualityOilBrakeDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }
                                var leakTester = leakTesterConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (leakTester != null)
                                {
                                    listQuality.LeakTester = Convert.ToDecimal(leakTester.Value);
                                }
                                var volumeOil = volumeOilConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (volumeOil != null)
                                {
                                    listQuality.VolumeOilBrake = Convert.ToDecimal(volumeOil.Value);
                                }
                                var errorCode = codeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (errorCode != null)
                                {
                                    listQuality.ErrorCode = Convert.ToInt32(errorCode.Value);
                                }
                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;
            }
            return dt;
        }

        public async Task<List<GetListQualityPressConeRaceDto>> GetAllListQualityPressConeRace(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityPressConeRaceDto> dt = new List<GetListQualityPressConeRaceDto>();
            var data = new GetListQualityPressConeRaceDto();

            var kedalaman = machine.Where(m => m.Subject.Vid.Contains("DEPTH")).FirstOrDefault();
            var tonase = machine.Where(m => m.Subject.Vid.Contains("TONASE")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var kedalamanConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = kedalaman.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var tonaseConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                            new { vid = tonase.Subject.Vid, starttime = start.Date, endtime = end.Date });
                        if (kedalamanConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityPressConeRaceDto
                            {
                                DateTime = DateTime.Now,
                                Kedalaman = 0,
                                Tonase = 0
                            };
                        }
                        else
                        {
                            foreach (var s in kedalamanConsumption)
                            {
                                GetListQualityPressConeRaceDto listQuality = new GetListQualityPressConeRaceDto();

                                var Depth = tonaseConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Depth != null)
                                {
                                    listQuality.Kedalaman = Convert.ToDecimal(s.Value);
                                }
                                var Tonasee = kedalamanConsumption.Where(k => k.Bucket == Depth.Bucket).FirstOrDefault();
                                if (Tonasee != null)
                                {
                                    listQuality.Tonase = Convert.ToDecimal(Tonasee.Value);
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                default:
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var kedalamanConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = kedalaman.Subject.Vid, now = DateTime.Now.Date });

                        var tonaseConsumption = await _dapperReadDbConnection.QueryAsync<PressConeRaceConsumption>
                        (@"SELECT * FROM ""list_quality_press_cone_race"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = tonase.Subject.Vid, now = DateTime.Now.Date });

                        if (kedalamanConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityPressConeRaceDto
                            {
                                DateTime = DateTime.Now,
                                Kedalaman = 0,
                                Tonase = 0
                            };
                        }
                        else
                        {
                            foreach (var s in kedalamanConsumption)
                            {
                                GetListQualityPressConeRaceDto listQuality = new GetListQualityPressConeRaceDto();

                                var Depth = tonaseConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Depth != null)
                                {
                                    listQuality.Kedalaman = Convert.ToDecimal(s.Value);
                                }
                                var Tonasee = kedalamanConsumption.Where(k => k.Bucket == Depth.Bucket).FirstOrDefault();
                                if (Tonasee != null)
                                {
                                    listQuality.Tonase = Convert.ToDecimal(Tonasee.Value);
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;
            }

            return dt;
        }

        public async Task<List<GetListQualityRobotScanImageDto>> GetAllListQualityRobotScanImage(Guid machineId, string type, DateTime start, DateTime end)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
            .Where(m => (machineId == m.MachineId)).ToListAsync();

            List<GetListQualityRobotScanImageDto> dt = new List<GetListQualityRobotScanImageDto>();
            var data = new GetListQualityRobotScanImageDto();

            var bc = machine.Where(m => m.Subject.Vid.Contains("ID-PART")).FirstOrDefault();
            var status = machine.Where(m => m.Subject.Vid.Contains("STATUS-PRDCT")).FirstOrDefault();

            switch (type)
            {
                case "day":
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                        (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                        new { vid = bc.Subject.Vid, starttime = start.Date, endtime = end.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                            (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                        AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                        ORDER BY id DESC, bucket DESC",
                            new { vid = status.Subject.Vid, starttime = start.Date, endtime = end.Date });
                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityRobotScanImageDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                            };
                        }
                        else
                        {
                            foreach (var s in statusConsumption)
                            {
                                GetListQualityRobotScanImageDto listQuality = new GetListQualityRobotScanImageDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                    }
                    break;

                default:
                    if (end.Date < start.Date)
                    {
                        throw new ArgumentException("End day cannot be earlier than start date.");
                    }
                    else
                    {
                        var barcodeConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                        (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @now)
                        ORDER BY  bucket DESC",
                        new { vid = bc.Subject.Vid, now = DateTime.Now.Date });

                        var statusConsumption = await _dapperReadDbConnection.QueryAsync<RobotConsumption>
                                (@"SELECT * FROM ""list_quality_robot_scan_image_and_abs_tester"" WHERE id = @vid
                        AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                        ORDER BY  bucket DESC",
                                new { vid = status.Subject.Vid, dateNow = DateTime.Now.Date, });

                        if (statusConsumption.Count() == 0)
                        {
                            data =
                            new GetListQualityRobotScanImageDto
                            {
                                DateTime = DateTime.Now,
                                Status = "-",
                                DataBarcode = "-",
                            };
                        }
                        else
                        {
                            foreach (var s in statusConsumption)
                            {
                                GetListQualityRobotScanImageDto listQuality = new GetListQualityRobotScanImageDto();

                                var Barcode = barcodeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                                if (Barcode != null)
                                {
                                    listQuality.DataBarcode = Barcode.Value;
                                }

                                var statuss = statusConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                                if (statuss != null && statuss.Value.Contains("1"))
                                {
                                    listQuality.Status = "OK";
                                }
                                else
                                {
                                    listQuality.Status = "NG";
                                }
                                listQuality.DateTime = s.Bucket.AddHours(7);
                                dt.Add(listQuality);
                            }
                        }
                        break;
                    }
            }
            return dt;
        }
    }
}