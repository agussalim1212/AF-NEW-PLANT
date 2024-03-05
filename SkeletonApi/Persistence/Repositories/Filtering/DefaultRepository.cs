using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.MachinesInformation.DetailEnergyConsumptions.Queries;
using SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AirConsumptionDetailMachine;
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

        public async Task<GetAllDetailMachineAirAndElectricConsumptionDto> GetAllDetailMachineAirAndElectricConsumptionAsync(string view, string vid, string machineName, string subjectName)
        {
            var setting = _repositorySetting.FindByCondition(o => o.MachineName == machineName && o.SubjectName == subjectName).FirstOrDefault();
            var data = new GetAllDetailMachineAirAndElectricConsumptionDto();
     
                var airConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumptionDetail>
                ($@"SELECT * FROM {view} WHERE id = @id
                AND date_trunc('week', bucket) = date_trunc('week', now()) 
                ORDER BY bucket DESC", new { id = vid });

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
                             Label = val.Bucket.AddHours(7).ToString("ddd"),
                             DateTime = val.Bucket,
                         }).OrderByDescending(x => x.DateTime).ToList()

                     };
                }
                return data;
        }

        public async Task<GetAllDetailMachineEnergyConsumptionDto> GetAllDetailMachineEnergyConsumptionAsync(string vid, string machineName, string subjectName)
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

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelPressBearing(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            var statusPressBearing = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("BRNG-STATUS-PRDCT")).Include(o => o.Subject).FirstOrDefault();
            var brngDistance = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("DISTANCE")).Include(o => o.Subject).FirstOrDefault();
            var brngTonase = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("TONASE")).Include(o => o.Subject).FirstOrDefault();

            var pressbearingDistnaceConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                           (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                           AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                           ORDER BY  bucket DESC",
                           new { vid = brngDistance.Subject.Vid, dateNow = DateTime.Now.Date, });

            var pressbearingTonaseConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                           AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                           ORDER BY  bucket DESC",
            new { vid = brngTonase.Subject.Vid, dateNow = DateTime.Now.Date, });

            var statusBearingConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                           AND date_trunc('day', bucket::date) = date_trunc('day', @dateNow)
                           ORDER BY  bucket DESC",
            new { vid = statusPressBearing.Subject.Vid, dateNow = DateTime.Now.Date, });

            if (statusBearingConsumption.Count() == 0)
            {
                data =
                new GetListWheelRearDto
                {
                    DateTime = DateTime.Now,
                    Status = "-",
                    DataDistance = 0,
                    DataTonase = 0,
                };
                dt.Add(data);
            }
            else
            {

                foreach (var s in statusBearingConsumption)
                {
                    GetListWheelRearDto listQuality = new GetListWheelRearDto();

                    var dataDial = pressbearingDistnaceConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataDial != null)
                    {
                        listQuality.DataDistance = Convert.ToDecimal(dataDial.Value);
                    }
                    var dataTonase = pressbearingTonaseConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                    if (dataTonase != null)
                    {
                        listQuality.DataTonase = Convert.ToDecimal(dataTonase.Value);
                    }
                    var statuss = statusBearingConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
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
            return dt;
        }
    }
}
