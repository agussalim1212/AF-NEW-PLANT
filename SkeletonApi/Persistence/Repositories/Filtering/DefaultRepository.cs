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
    public class DefaultRepository : IDefaultRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenericRepository<Setting> _repositorySetting;
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectHasMachine;

        public DefaultRepository(IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Setting> repositorySetting, IGenRepository<SubjectHasMachine> repositorySubjectHasMachine)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _repositorySetting = repositorySetting;
            _repositorySubjectHasMachine = repositorySubjectHasMachine;
        }

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionDefault(string view, string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineAirAndElectricConsumptionDto();
            DateTime today = DateTime.Now.Date;
            var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
            ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('week', bucket) = date_trunc('week', @today)
                ORDER BY bucket DESC", new { id = vid, today });

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
                     Data = airConsumption.Select(val => new DataAir
                     {
                         Value = val.ValueLast - val.ValueFirst,
                         Label = val.Bucket.ToString("ddd"),
                         DateTime = val.Bucket,
                     }).OrderByDescending(x => x.DateTime).ToList()
                 };
            }
            return data;
        }

        public async Task<GetAllDetailMachineCurrentAndVoltageConsumptionDto> GetAllDetailMachineCurrentAndVoltageConsumptionDefault(string view, string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto();
            DateTime today = DateTime.Now.Date;
            var currentConsumptions = await _dapperReadDbConnection.QueryAsync<CurrentConsumptions>
                ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('week', bucket) = date_trunc('week', @today)
                ORDER BY bucket DESC", new { id = vid, today });

            var totals = currentConsumptions.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
            {
                date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                last = g.Select(p => p.LastValue).First()
            }).ToList();

            if (currentConsumptions.Count() == 0)
            {
                data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    Maximum = setting == null ? null : setting.Maximum,
                    Medium = setting == null ? null : setting.Medium,
                    Minimum = setting == null ? null : setting.Minimum,
                };
            }
            else
            {
                data = new GetAllDetailMachineCurrentAndVoltageConsumptionDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    Maximum = setting == null ? null : setting.Maximum,
                    Medium = setting == null ? null : setting.Medium,
                    Minimum = setting == null ? null : setting.Minimum,
                    Data = totals.Select(val => new Data
                    {
                        Value = val.last,
                        Messages = setting == null ? null :
                                   val.last > setting.Maximum ? $"WARNING! VALUE IS {val.last}" :
                                   val.last < setting.Minimum ? $"WARNING! VALUE IS {val.last}" :
                                        null,
                Label = val.date_time.ToString("ddd"),
                        DateTime = val.date_time,
                    }).OrderByDescending(x => x.DateTime).ToList()
                };

                //data =
                // new GetAllDetailMachineCurrentAndVoltageConsumptionDto
                // {
                //     MachineName = machineName,
                //     SubjectName = subjectName,
                //     Maximum = setting == null ? null : setting.Maximum,
                //     Medium = setting == null ? null : setting.Medium,
                //     Minimum = setting == null ? null : setting.Minimum,
                //     Data = totals.Select(val => new Data
                //     {
                //         Value = val.last,
                //         Messages = val.last > setting.Maximum ? $"ABNORMAL VALUE, CURRENT VALUE IS {val.last} IN CONV MAIN LINE CURRENT" : $"NORMAL VALUE, CURRENT VALUE IS {val.last} IN CONV MAIN LINE CURRENT",
                //         Label = val.date_time.ToString("ddd"),
                //         DateTime = val.date_time,
                //     }).OrderByDescending(x => x.DateTime).ToList()
                // };
            }
            return data;
        }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionDefault(string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineEnergyConsumptionDto();
            DateTime today = DateTime.Now.Date;
            var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @id
                AND date_trunc('week', bucket) = date_trunc('week', @today)
                ORDER BY bucket DESC", new { id = vid, today });

            var totals = energyConsumptions.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day }).Select(g => new
            {
                date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                last = g.Sum(k => k.ValueLast),
                first = g.Select(p => p.ValueFirst).First()
            }).ToList();

            if (energyConsumptions.Count() == 0)
            {
                data = new GetAllDetailMachineEnergyConsumptionDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                };
            }
            else
            {
                data =
                 new GetAllDetailMachineEnergyConsumptionDto
                 {
                     MachineName = machineName,
                     SubjectName = subjectName,
                     //Maximum = setting.Maximum,
                     //Medium = setting.Medium,
                     //Minimum = setting.Minimum,
                     Data = totals.Select(val => new DataPower
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

        public async Task<List<GetAllDetailEnergyConsumptionDto>> GetAllEnergyConsumptionSummary()
        {
            var subjectMachine = _repositorySubjectHasMachine.FindByCondition(x => x.Subject.Vid.Contains("POWER-CONSUMPTION")).Include(o => o.Subject).Include(p => p.Machine);
            var subjects = subjectMachine.Select(o => o.Subject.Vid).ToList();
            var setting = _repositorySetting.FindByCondition(o => o.SubjectName == "POWER CONSUMPTION ALL").FirstOrDefault();

            List<GetAllDetailEnergyConsumptionDto> dt = new List<GetAllDetailEnergyConsumptionDto>();
            var data = new GetAllDetailEnergyConsumptionDto();
            DateTime today = DateTime.Now.Date;
            var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
            (@"SELECT * FROM ""power_consumption_setting"" WHERE id = ANY(@id)
                AND date_trunc('week', bucket) = date_trunc('week', @today)
                ORDER BY bucket DESC", new { id = subjects, today });

            var Groups = energyConsumptions.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month, p.Bucket.Day })
            .Select(g => new
            {
                date_time = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                totalFirst = g.Sum(k => k.ValueFirst),
                totalLast = g.Sum(k => k.ValueLast)
            }).ToList();

            if (energyConsumptions.Count() == 0)
            {
                data = new GetAllDetailEnergyConsumptionDto
                {
                    Label = ""
                };
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
            return dt;
        }

        public async Task<GetAllTotalProductionDto> GetAllTotalProductionDefault(Guid machineId, string vidOK, string vidNG, string machineName)
        {
            var data = new GetAllTotalProductionDto();
            DateTime today = DateTime.Now.Date.AddHours(7);
            var productConsumption = await _dapperReadDbConnection.QueryAsync<ProductConsumption>
                    (@"SELECT * FROM ""production_consumption"" WHERE id = @vidok OR id = @vidng
                    AND date_trunc('day', bucket::date) = date_trunc('day', @today)",
                    new { vidok = vidOK, vidng = vidNG, today });

            decimal valueOK = productConsumption.Where(k => k.Id.Contains(vidOK)).Sum(o => Convert.ToDecimal(o.LastValue));
            decimal valueNG = productConsumption.Where(k => k.Id.Contains(vidNG)).Sum(o => Convert.ToDecimal(o.LastValue));

            if (productConsumption.Count() == 0)
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
                    ValueOkTotal = valueOK,
                    ValueNgTotal = valueNG,
                    ValueOKPresentase = Math.Round((valueOK / (valueOK + valueNG)) * 100, 2),
                    ValueNgPresentase = Math.Round((valueNG / (valueNG + valueOK)) * 100, 2),
                };
            }
            return data;
        }

        public async Task<List<GetListWheelFrontDto>> GetListQualityAssyWheelFrontDiskBrake(string vidTorsi)
        {
            List<GetListWheelFrontDto> dt = new List<GetListWheelFrontDto>();
            var data = new GetListWheelFrontDto();
            DateTime today = DateTime.Now.Date;
            var TireConsumption = await _dapperReadDbConnection.QueryAsync<WheelFrontConsumption>
            (@"SELECT * FROM ""list_quality_wheel_front_db"" WHERE id = @id
            AND date_trunc('day', bucket::date) = date_trunc('day', @today)
            ORDER BY bucket DESC",
            new { id = vidTorsi, today });

            if (TireConsumption.Count() == 0)
            {
                data =
                new GetListWheelFrontDto
                {
                    DateTime = null,
                    Status = null,
                    DataDistance = null,
                    DataTonase = null,
                    DataDialHorizontal = null,
                    DataDialVertical = null,
                    DiskBrake = null,
                    DataTorQ = null,
                    TirePresure = null,
                };
                dt.Add(data);
            }
            else
            {
                foreach (var s in TireConsumption)
                {
                    GetListWheelFrontDto listQuality = new GetListWheelFrontDto();

                    listQuality.DiskBrake = s.Value;
                    listQuality.DateTime = s.Bucket.ToString("dd-MM-yyy HH:mm:ss");
                    dt.Add(listQuality);
                }
            }
            return dt;
        }

        public async Task<List<GetListWheelFrontDto>> GetListQualityAssyWheelFrontTireInflationDefault(string vid)
        {
            List<GetListWheelFrontDto> dt = new List<GetListWheelFrontDto>();
            var data = new GetListWheelFrontDto();
            DateTime today = DateTime.Now.Date;
            var TireConsumption = await _dapperReadDbConnection.QueryAsync<WheelFrontConsumption>
            (@"SELECT * FROM ""list_quality_wheel_front_ti"" WHERE id = @id
            AND date_trunc('day', bucket::date) = date_trunc('day', @today)
            ORDER BY bucket DESC",
            new { id = vid, today });

            if (TireConsumption.Count() == 0)
            {
                data =
                new GetListWheelFrontDto
                {
                    DateTime = null,
                    Status = null,
                    DataDistance = null,
                    DataTonase = null,
                    DataDialHorizontal = null,
                    DataDialVertical = null,
                    DiskBrake = null,
                    DataTorQ = null,
                    TirePresure = null,
                };
                dt.Add(data);
            }
            else
            {
                foreach (var s in TireConsumption)
                {
                    GetListWheelFrontDto listQuality = new GetListWheelFrontDto();

                    listQuality.TirePresure = s.Value;
                    listQuality.DateTime = s.Bucket.ToString("dd-MM-yyy HH:mm:ss");
                    dt.Add(listQuality);
                }
            }
            return dt;
        }

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearFinalInspectionDefault(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            var horizontalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @id
            AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
            ORDER BY bucket DESC",
            new { id = vidHorizontal, dateNow = DateTime.Now.Date, });

            var vertikalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @id
            AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
            ORDER BY bucket DESC",
            new { id = vidVertikal, dateNow = DateTime.Now.Date, });

            var statusInspectionConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @id
            AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
            ORDER BY bucket DESC",
            new { id = vidStatus, dateNow = DateTime.Now.Date, });

            var diskBrakeConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_fi"" WHERE id = @id
            AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
            ORDER BY bucket DESC",
            new { id = vidDiskBrake, dateNow = DateTime.Now.Date, });

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

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearTireInflationDefault(string vid)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            var TireConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear_ti"" WHERE id = @id
            AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
            ORDER BY bucket DESC",
            new { id = vid, dateNow = DateTime.Now.Date, });

            if (TireConsumption.Count() == 0)
            {
                data =
                new GetListWheelRearDto
                {
                    DateTime = null,
                    Status = null,
                    DataDistance = null,
                    DataTonase = null,
                };
                dt.Add(data);
            }
            else
            {
                foreach (var s in TireConsumption)
                {
                    GetListWheelRearDto listQuality = new GetListWheelRearDto();

                    listQuality.TirePresure = s.Value;
                    listQuality.DateTime = s.Bucket.ToString("dd-MM-yyy HH:mm:ss");
                    dt.Add(listQuality);
                }
            }
            return dt;
        }
    }
}