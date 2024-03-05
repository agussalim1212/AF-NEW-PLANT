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

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelDiskBrake(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End)
        {
           
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();
            //disk brake
            var torQ = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("TORQ")).Include(p => p.Subject).FirstOrDefault();

            var torQConsumption = await _dapperReadDbConnection.QueryAsync <WheelRearConsumption>
                          (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                          AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                          AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                          ORDER BY id DESC, bucket DESC", new { vid = torQ.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

            if (torQConsumption.Count() == 0)
            {
                data =
                new GetListWheelRearDto
                {
                    DateTime = DateTime.Now,
                    DataTorQ = 0
                };
                dt.Add(data);
            }
            else
            {
                foreach (var s in torQConsumption)
                {
                    GetListWheelRearDto listQuality = new GetListWheelRearDto();

                    listQuality.DataTorQ = Convert.ToDecimal(s.Value);
                    listQuality.DateTime = s.Bucket.AddHours(7);
                    dt.Add(listQuality);
                }
            }
            return dt;
        }

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelFinalInspection(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            //final inspection
            var statusInspection = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("FI-STATUS-PRODUKSI")).FirstOrDefault();
            var horizontal = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("FI-DIAL-HORIZONTAL")).FirstOrDefault();
            var vertikal = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("FI-DIAL-VERTIKAL")).FirstOrDefault();
            var diskBrake = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("FI-DISK-BRAKE")).FirstOrDefault();


                    var horizontalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = horizontal.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

                    var vertikalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = vertikal.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

                    var statusInspectionConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = statusInspection.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

                    var diskBrakeConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
                    (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                                AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                                AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                                ORDER BY id DESC, bucket DESC", new { vid = diskBrake.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

                    if (statusInspectionConsumption.Count() == 0)
                    {
                        data =
                        new GetListWheelRearDto
                        {
                            DateTime = DateTime.Now,
                            Status = "-",
                            DataDialHorizontal = 0,
                            DataDialVertical = 0,
                        };
                        dt.Add(data);
                    }
                    else
                    {
                        foreach (var s in statusInspectionConsumption)
                        {
                            GetListWheelRearDto listQuality = new GetListWheelRearDto();

                            var dataDialHorizontal = horizontalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                            if (dataDialHorizontal != null)
                            {
                                listQuality.DataDialHorizontal = Convert.ToDecimal(dataDialHorizontal.Value);
                            }
                            var dataDialVertikal = vertikalConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                            if (dataDialVertikal != null)
                            {
                                listQuality.DataDialVertical = Convert.ToDecimal(dataDialVertikal.Value);
                            }
                            var dataDiskBrake = diskBrakeConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
                            if (dataDiskBrake != null)
                            {
                                listQuality.DiskBrake = Convert.ToDecimal(dataDiskBrake.Value);
                            }
                            var statuss = statusInspectionConsumption.Where(g => g.Bucket == s.Bucket).FirstOrDefault();
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

        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelTireInflation(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            var tire = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("TIRE-PRESURE")).FirstOrDefault();

            var horizontalConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
            AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
            AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
            ORDER BY id DESC, bucket DESC", new { vid = tire.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

            if (horizontalConsumption.Count() == 0)
            {
                data =
                new GetListWheelRearDto
                {
                    DateTime = DateTime.Now,
                    Status = "-",
                    TirePresure = 0,
                };
                dt.Add(data);
            }
            else
            {
                foreach (var s in horizontalConsumption)
                {
                    GetListWheelRearDto listQuality = new GetListWheelRearDto();

                    listQuality.DataTorQ = Convert.ToDecimal(s.Value);
                    listQuality.DateTime = s.Bucket.AddHours(7);
                    dt.Add(listQuality);
                }
            }
            return dt;
        }
        public async Task<List<GetListWheelRearDto>> GetListQualityAssyWheelPressBearing(string typesWheel, string searchTerm, Guid machineId, DateTime? Start, DateTime? End)
        {
            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            //press bearing
            var statusPressBearing = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("BRNG-STATUS-PRDCT")).FirstOrDefault();
            var brngDistance = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("DISTANCE")).FirstOrDefault();
            var brngTonase = _repositorySubjectHasMachine.FindByCondition(m => m.Subject.Vid.Contains("TONASE")).FirstOrDefault();

            var pressbearingDistanceConsumption = await _dapperReadDbConnection.QueryAsync < WheelRearConsumption >
            (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                           AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                           AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                           ORDER BY id DESC, bucket DESC", new { vid = brngDistance.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

            var pressbearingTonaseConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                           AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                           AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                           ORDER BY id DESC, bucket DESC", new { vid = brngTonase.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

            var statusBearingConsumption = await _dapperReadDbConnection.QueryAsync<WheelRearConsumption>
            (@"SELECT * FROM ""list_quality_wheel_rear"" WHERE id = @vid
                           AND date_trunc('day', bucket) >= date_trunc('day', @starttime::date)
                           AND date_trunc('day', bucket) <= date_trunc('day', @endtime::date)
                           ORDER BY id DESC, bucket DESC", new { vid = statusPressBearing.Subject.Vid, starttime = Start.Value.Date, endtime = End.Value.Date });

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

                    var dataDial = pressbearingDistanceConsumption.Where(k => k.Bucket == s.Bucket).FirstOrDefault();
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
