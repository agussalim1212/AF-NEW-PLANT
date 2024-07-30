using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.DTOs.AirAndElectricConsumption;
using SkeletonApi.Application.DTOs.Consumption;
using SkeletonApi.Application.DTOs.CurrentAndVoltageConsumption;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumption.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.TotalProduction;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Configuration;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Persistence.Repositories.Filtering
{
    public class DayRepository : IDayRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenericRepository<Setting> _repositorySetting;
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectHasMachine;

        public DayRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting, IGenRepository<SubjectHasMachine> repositorySubjectHasMachine)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
            _repositorySubjectHasMachine = repositorySubjectHasMachine;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineAirAndElectricConsumptionDto();

            if (endTime.Value < startTime.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                ORDER BY bucket DESC",
                new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });

                var total = airConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    last = g.Sum(k => k.ValueLast),
                    first = g.Sum(p => p.ValueFirst)
                }).ToList();

                if (airConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineAirAndElectricConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                    };
                }
                else
                {
                    data =
                     new GetAllDetailMachineAirAndElectricConsumptionDto
                     {
                         MachineName = machineName,
                         SubjectName = subjectName,
                         //Maximum = setting.Maximum,
                         //Medium = setting.Medium,
                         //Minimum = setting.Minimum,
                         Data = total.Select(val => new DataAir
                         {
                             Value = val.last - val.first,
                             Label = val.date_time.ToString("ddd"),
                             DateTime = val.date_time,
                         }).OrderByDescending(x => x.DateTime).ToList()
                     };
                }
                return data;
            }
        }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionAsync(string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineEnergyConsumptionDto();
            if (endTime.Value < startTime.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var energyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @id
                 AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                 AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                 ORDER BY bucket DESC",
                new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });

                var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day })
                .Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    last = g.Sum(k => k.ValueLast),
                    first = g.Sum(p => p.ValueFirst),
                }).ToList();

                if (energyConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineEnergyConsumptionDto();
                }
                else
                {
                    data = new GetAllDetailMachineEnergyConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        //Maximum = setting.Maximum,
                        //Medium = setting.Medium,
                        //Minimum = setting.Minimum,
                        Data = total.Select(val => new DataPower
                        {
                            ValueKwh = val.last - val.first,
                            ValueCo2 = Math.Round((val.last - val.first) * Convert.ToDecimal(0.87), 2),
                            Label = val.date_time.ToString("ddd"),
                            DateTime = val.date_time,
                        }).OrderByDescending(x => x.DateTime).ToList()
                    };
                }
                return data;
            }
        }

        public async Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary(DateTime? start, DateTime? end)
        {
            var subjectMachine = _repositorySubjectHasMachine.FindByCondition(x => x.Subject.Vid.Contains("POWER-CONSUMPTION")).Include(o => o.Subject).Include(p => p.Machine);
            var subjects = subjectMachine.Select(o => o.Subject.Vid).ToList();
            var setting = _repositorySetting.FindByCondition(o => o.SubjectName == "POWER CONSUMPTION ALL").FirstOrDefault();

            List<GetAllDetailEnergyConsumptionDto> dt = new List<GetAllDetailEnergyConsumptionDto>();
            var data = new GetAllDetailEnergyConsumptionDto();

            if (end.Value < start.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var EnergyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = ANY(@vid)
                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                ORDER BY bucket DESC",
                new { vid = subjects, starttime = start.Value.Date, endtime = end.Value.Date });

                var Groups = EnergyConsumptions.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day })
                .Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    totalFirst = g.Sum(k => k.ValueFirst),
                    totalLast = g.Sum(k => k.ValueLast)
                }).ToList();

                if (EnergyConsumptions.Count() == 0)
                {
                    dt.Add(data);
                }
                else
                {
                    dt = Groups.Select(o => new GetAllDetailEnergyConsumptionDto
                    {
                        ValueKwh = o.totalLast - o.totalFirst,
                        ValueCo2 = Math.Round((o.totalLast - o.totalFirst) * Convert.ToDecimal(0.87), 2),
                        //Maximum = setting.Maximum,
                        //Medium = setting.Medium,
                        //Minimum = setting.Minimum,
                        Label = o.date_time.ToString("ddd").ToString(),
                        DateTime = o.date_time,
                    }).ToList();
                }
            }
            return dt;
        }

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearFinalInspectioDay(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake, Guid machineId, DateTime? Start, DateTime? End)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            var horizontalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = vidHorizontal, starttime = Start.Value.Date, endtime = End.Value.Date });

            var vertikalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = vidVertikal, starttime = Start.Value.Date, endtime = End.Value.Date });

            var statusInspectionConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = vidStatus, starttime = Start.Value.Date, endtime = End.Value.Date });

            var diskBrakeConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = vidDiskBrake, starttime = Start.Value.Date, endtime = End.Value.Date });

            var allConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                                (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE
                                date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY bucket DESC", new { starttime = Start.Value.Date, endtime = End.Value.Date });

            if (horizontalConsumption.Count() == 0 || vertikalConsumption.Count() == 0)
            {
                dt = new List<GetListWheelRearDto>();
            }
            else
            {
                foreach (var s in vertikalConsumption)
                {
                    GetListWheelRearDto listQuality = new GetListWheelRearDto();

                    var dataDialHorizontal = horizontalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataDialHorizontal != null)
                    {
                        listQuality.DataDialHorizontal = dataDialHorizontal.Value;
                    }
                    else
                    {
                        listQuality.DataDialHorizontal = "0";
                    }
                    var dataDialVertikal = vertikalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataDialVertikal != null)
                    {
                        listQuality.DataDialVertical = dataDialVertikal.Value;
                    }
                    else
                    {
                        listQuality.DataDialVertical = "0";
                    }
                    var statuss = statusInspectionConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                    if (statuss != null && statuss.Value.Contains("1"))
                    {
                        listQuality.Status = "OK";
                    }
                    else if (statuss == null)
                    {
                        listQuality.Status = "-";
                    }
                    else
                    {
                        listQuality.Status = "NG";
                    }
                    var dataDiskBrake = diskBrakeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataDiskBrake != null)
                    {
                        listQuality.DiskBrake = dataDiskBrake.Value;
                    }
                    else
                    {
                        listQuality.DiskBrake = "0";
                    }

                    listQuality.DateTime = s.Bucket.ToString("dd-MM-yyy HH:mm:ss");

                    dt.Add(listQuality);
                }
            }
            return dt;
        }

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearTireInflationDay(string vid, string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();

            var horizontalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_ti"" WHERE id = @id
            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
            ORDER BY id DESC, bucket DESC", new { id = vid, starttime = Start.Value.Date, endtime = End.Value.Date });

            if (horizontalConsumption.Count() == 0)
            {
                dt = new List<GetListWheelRearDto>();
            }
            else
            {
                foreach (var s in horizontalConsumption)
                {
                    GetListWheelRearDto listQuality = new GetListWheelRearDto();

                    listQuality.DataTorQ = s.Value;
                    listQuality.DateTime = s.Bucket.ToString("dd-MM-yyy HH:mm:ss");
                    dt.Add(listQuality);
                }
            }
            return dt;
        }

        public async Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionDay(string view, string vid, string machineName, string subjectName, DateTime? startTime, DateTime? endTime)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto();
            if (endTime.Value < startTime.Value)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var energyConsumption = await _dapperReadDbConnection.QueryAsync<CurrentConsumptions>
                ($@"SELECT * FROM {view} WHERE id = @id
                 AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                 AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                 ORDER BY bucket DESC"
                ,
                new { id = vid, starttime = startTime.Value.Date, endtime = endTime.Value.Date });

                var total = energyConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day })
                .Select(g => new
                {
                    date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    last = g.Select(k => k.LastValue).First(),
                }).ToList();

                if (energyConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto();
                }
                else
                {
                    data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                        //Maximum = setting.Maximum,
                        //Medium = setting.Medium,
                        //Minimum = setting.Minimum,
                        Data = total.Select(val => new Data
                        {
                            Value = val.last,
                            Label = val.date_time.ToString("ddd"),
                            DateTime = val.date_time,
                        }).OrderByDescending(x => x.DateTime).ToList()
                    };
                }
                return data;
            }
        }

        public async Task<List<GetListWheelFrontDto>> GetListQualityWheelFrontFinalInspectionDay(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake, Guid machineId, DateTime? Start, DateTime? End)
        {
            List<GetListWheelFrontDto> dt = new List<GetListWheelFrontDto>();
            var data = new GetListWheelFrontDto();

            var horizontalConsumption = await _dapperReadDbConnection.QueryAsync<WheelFrontConsumption>
            (@"SELECT * FROM ""list_quality_wheel_front_fi"" WHERE id = @vid
                     AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                     AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                     ORDER BY id DESC, bucket DESC", new { vid = vidHorizontal, starttime = Start.Value.Date, endtime = End.Value.Date });

            var vertikalConsumption = await _dapperReadDbConnection.QueryAsync<WheelFrontConsumption>
            (@"SELECT * FROM ""list_quality_wheel_front_fi"" WHERE id = @vid
                     AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                     AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                     ORDER BY id DESC, bucket DESC", new { vid = vidVertikal, starttime = Start.Value.Date, endtime = End.Value.Date });

            var statusInspectionConsumption = await _dapperReadDbConnection.QueryAsync<WheelFrontConsumption>
            (@"SELECT * FROM ""list_quality_wheel_front_fi"" WHERE id = @vid
                     AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                     AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                     ORDER BY id DESC, bucket DESC", new { vid = vidStatus, starttime = Start.Value.Date, endtime = End.Value.Date });

            var diskBrakeConsumption = await _dapperReadDbConnection.QueryAsync<WheelFrontConsumption>
            (@"SELECT * FROM ""list_quality_wheel_front_fi"" WHERE id = @vid
                     AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                     AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                     ORDER BY id DESC, bucket DESC", new { vid = vidDiskBrake, starttime = Start.Value.Date, endtime = End.Value.Date });

            if (horizontalConsumption.Count() == 0 || vertikalConsumption.Count() == 0)
            {
                dt = new List<GetListWheelFrontDto>();
            }
            else
            {
                foreach (var s in vertikalConsumption)
                {
                    GetListWheelFrontDto listQuality = new GetListWheelFrontDto();

                    var dataDialHorizontal = horizontalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataDialHorizontal != null)
                    {
                        listQuality.DataDialHorizontal = dataDialHorizontal.Value;
                    }
                    else
                    {
                        listQuality.DataDialHorizontal = "0";
                    }
                    var dataDialVertikal = vertikalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataDialVertikal != null)
                    {
                        listQuality.DataDialVertical = dataDialVertikal.Value;
                    }
                    else
                    {
                        listQuality.DataDialVertical = "0";
                    }
                    var statuss = statusInspectionConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
                    if (statuss != null && statuss.Value.Contains("1"))
                    {
                        listQuality.Status = "OK";
                    }
                    else if (statuss == null)
                    {
                        listQuality.Status = "-";
                    }
                    else
                    {
                        listQuality.Status = "NG";
                    }
                    var dataDiskBrake = diskBrakeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataDiskBrake != null)
                    {
                        listQuality.DiskBrake = dataDiskBrake.Value;
                    }
                    else
                    {
                        listQuality.DiskBrake = "0";
                    }

                    listQuality.DateTime = s.Bucket.ToString("dd-MM-yyy HH:mm:ss");

                    dt.Add(listQuality);
                }
            }
            return dt;
        }

        public async Task<GetAllTotalProductionDto> GetAllTotalProductionDay(Guid machineId, string vidOK, string vidNG, string machineName, DateTime? start, DateTime? end)
        {
            var data = new GetAllTotalProductionDto();
            if (end.Value.Date < start.Value.Date)
            {
                throw new ArgumentException("End day cannot be earlier than start date.");
            }
            else
            {
                var productionDay = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                (@"SELECT * FROM ""production_consumption"" WHERE id = @vidok OR id = @vidng
                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)",
                new { vidok = vidOK, vidng = vidNG, starttime = start.Value.Date, endtime = end.Value.Date });

                decimal TotalOk = productionDay.Where(p => p.Id.Contains(vidOK)).Sum(o => Convert.ToDecimal(o.LastValue));
                decimal TotalNg = productionDay.Where(p => p.Id.Contains(vidNG)).Sum(o => Convert.ToDecimal(o.LastValue));

                if (productionDay.Count() == 0)
                {
                    data = new GetAllTotalProductionDto
                    {
                        MachineName = machineName
                    };
                }
                else
                {
                    data =
                    new GetAllTotalProductionDto
                    {
                        MachineName = machineName,
                        ValueOkTotal = TotalOk,
                        ValueNgTotal = TotalNg,
                        ValueOKPresentase = Math.Round((TotalOk / (TotalOk + TotalNg)) * 100, 2),
                        ValueNgPresentase = Math.Round((TotalNg / (TotalNg + TotalOk)) * 100, 2),
                    };
                }
            }

            return data;
        }
    }
}