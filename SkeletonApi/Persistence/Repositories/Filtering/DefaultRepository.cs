using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AmpereConsumptionDetailMachine;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.EnergyConsumption;
using SkeletonApi.Application.Interfaces.Repositories;
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

        public async Task<GetAllDetailMachineAirConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionDefault(string view, string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineAirConsumptionDto();
     
                var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('week', bucket) = date_trunc('week', now()) 
                ORDER BY bucket DESC", new { id = vid });

                if (airConsumption.Count() == 0)
                {
                    data = new GetAllDetailMachineAirConsumptionDto
                    {
                        MachineName = machineName,
                        SubjectName = subjectName,
                    };
                }
                else
                {

                    data =
                     new GetAllDetailMachineAirConsumptionDto
                     {
                         MachineName = machineName,
                         SubjectName = subjectName,
                         //Maximum = setting.Maximum,
                         //Medium = setting.Medium,
                         //Minimum = setting.Minimum,
                         Data = airConsumption.Select(val => new DataAir
                         {
                             Value = val.ValueLast - val.ValueFirst,
                             Label = val.Bucket.AddHours(7).ToString("ddd"),
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
            var currentConsumptions = await _dapperReadDbConnection.QueryAsync<CurrentConsumptions>
                ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('week', bucket) = date_trunc('week', now()) 
                ORDER BY bucket DESC", new { id = vid });

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
                };
            }
            else
            {

                data =
                 new GetAllDetailMachineCurrentAndVoltageConsumptionDto
                 {
                     MachineName = machineName,
                     SubjectName = subjectName,
                     //Maximum = setting.Maximum,
                     //Medium = setting.Medium,
                     //Minimum = setting.Minimum,
                     Data = totals.Select(val => new Data
                     {
                         Value = val.last,
                         Label = val.date_time.AddHours(7).ToString("ddd"),
                         DateTime = val.date_time,
                     }).OrderByDescending(x => x.DateTime).ToList()
                 };
            }
            return data;
        }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionDefault(string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineEnergyConsumptionDto();
            var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = @id
                AND date_trunc('week', bucket) = date_trunc('week', now()) 
                ORDER BY bucket DESC", new { id = vid });

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
                         Label = val.date_time.AddHours(7).ToString("ddd"),
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

                var energyConsumptions = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting"" WHERE id = ANY(@id)
                AND date_trunc('week', bucket) = date_trunc('week', now()) 
                ORDER BY bucket DESC", new { id = subjects });

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

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearFinalInspectionDefault(string vidStatus, string vidHorizontal, string vidVertikal, string vidDiskBrake, Guid machineId, DateTime? Start, DateTime? End)
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

                    listQuality.DateTime = s.Bucket.AddHours(7).ToString("dd-MM-yyy HH:mm:ss");

                    dt.Add(listQuality);

                }
              
            }
            return dt;
        }

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelRearTireInflationDefault(string vid, string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End)
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
