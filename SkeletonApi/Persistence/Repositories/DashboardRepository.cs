using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Dashboard.FiveTopAirConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopEnergyConsumption.Queries;
using SkeletonApi.Application.Features.Dashboard.FiveTopMachineMaintenance.Queries;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Interfaces;
using System.Globalization;

namespace SkeletonApi.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IGenRepository<SubjectHasMachine> _repositorySubjectHasMachine;

        public DashboardRepository(IDapperReadDbConnection dapperReadDb, IGenRepository<SubjectHasMachine> repositorySubjectHasMachine)
        {
            _dapperReadDbConnection = dapperReadDb;
            _repositorySubjectHasMachine = repositorySubjectHasMachine;
        }

        public async Task<GetAllTop5AirConsumptionsDto> GetAllTop5AirConsumptionsAsync()
        {
            var subjectMachine = _repositorySubjectHasMachine.FindByCondition(x => x.Subject.Vid.Contains("AIR-CONSUMPTION")).Include(o => o.Subject).Include(p => p.Machine);
            var subjects = subjectMachine.Select(o => o.Subject.Vid).ToList();

            var data = new GetAllTop5AirConsumptionsDto();

            var EnergyConsumption = await _dapperReadDbConnection.QueryAsync<AirConsumption>
               (@"SELECT * FROM ""air_consumption_setting"" WHERE id = ANY(@vid)
               AND date_trunc('year', bucket::date) = date_trunc('year', now())
               ORDER BY bucket DESC",
               new { vid = subjects });

            var Groups = EnergyConsumption.GroupBy(p => new { p.Id })
                .Select(g => new
                {
                    id = g.Key.Id,
                    totalFirst = g.Sum(k => k.ValueFirst),
                    totalLast = g.Sum(k => k.ValueLast)
                }).ToList();


            var EnergyWeekConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""air_consumption_setting"" WHERE id = ANY(@vid)
                AND date_trunc('week', bucket::date) = date_trunc('week', now())
                ORDER BY bucket DESC",
                new { vid = subjects });

            var groupWeek = EnergyWeekConsumption.GroupBy(p => new
            {
                WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(p.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
            })
           .Select(g => new
           {
               date_time = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
               totalFirst = g.Sum(k => k.ValueFirst),
               totalLast = g.Sum(k => k.ValueLast)
           }).ToList();
            decimal week = groupWeek.Select(p => p.totalLast - p.totalFirst).FirstOrDefault();

            var EnergyMonthConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""air_consumption_setting"" WHERE id = ANY(@vid)
               AND date_trunc('month', bucket::date) = date_trunc('month', now())
               ORDER BY  bucket DESC",
                 new { vid = subjects });

            var groupMonth = EnergyMonthConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month })
           .Select(g => new
           {
               date_time = new DateTime(g.Key.Year, g.Key.Month, 1),
               totalFirst = g.Sum(k => k.ValueFirst),
               totalLast = g.Sum(k => k.ValueLast)
           }).ToList();
            var month = groupMonth.Select(p => p.totalLast - p.totalFirst).FirstOrDefault();

            if (EnergyConsumption.Count() == 0)
            {
                data =
                new GetAllTop5AirConsumptionsDto
                {
                    TotalWeek = 0,
                    TotalMonth = 0,
                    DataMachines = new List<DataMachines>(),
                };
            }
            else
            {
                data =
                new GetAllTop5AirConsumptionsDto
                {
                    TotalWeek = Math.Round((week), 2),
                    TotalMonth = Math.Round((month), 2),
                    DataMachines = Groups
                    .Select(val => new DataMachines
                    {
                        Label = subjectMachine.Where(k => val.id == k.Subject.Vid).Select(v => v.Machine.Name).FirstOrDefault(),
                        Value = val.totalLast - val.totalFirst,
                    }).OrderByDescending(v => v.Value).Take(5).ToList()
                };
            }
            return data;
        }

        public async Task<GetAllTop5EnergyConsumptionsDto> GetAllTop5EnergyConsumptionsAsync()
        {
            var subjectMachine = _repositorySubjectHasMachine.FindByCondition(x => x.Subject.Vid.Contains("POWER-CONSUMPTION")).Include(o => o.Subject).Include(p => p.Machine);
            var subjects = subjectMachine.Select(o => o.Subject.Vid).ToList();

            var data = new GetAllTop5EnergyConsumptionsDto();

            var EnergyConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
               (@"SELECT * FROM ""power_consumption_setting_ex"" WHERE id = ANY(@vid)
               AND date_trunc('year', bucket::date) = date_trunc('year', now())
               ORDER BY bucket DESC",
               new { vid = subjects });

            var Groups = EnergyConsumption.GroupBy(p => new { p.Id })
                .Select(g => new
                {
                    id = g.Key.Id,
                    totalFirst = g.Sum(k => k.ValueFirst),
                    totalLast = g.Sum(k => k.ValueLast)
                }).ToList();


            var EnergyWeekConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
                (@"SELECT * FROM ""power_consumption_setting_ex"" WHERE id = ANY(@vid)
                AND date_trunc('week', bucket::date) = date_trunc('week', now())
                ORDER BY bucket DESC",
                new { vid = subjects});

            var groupWeek = EnergyWeekConsumption.GroupBy(p => new
            {
                WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(p.Bucket, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
            })
           .Select(g => new
           {
               date_time = new DateTime(g.Key.WeekNumber, 1, 1).AddDays((g.Key.WeekNumber - 1) * 7),
               totalFirst = g.Sum(k => k.ValueFirst),
               totalLast = g.Sum(k => k.ValueLast)
           }).ToList();
           decimal week = groupWeek.Select(p => p.totalLast - p.totalFirst).FirstOrDefault();
            
           var EnergyMonthConsumption = await _dapperReadDbConnection.QueryAsync<EnergyConsumption>
               (@"SELECT * FROM ""power_consumption_setting_ex"" WHERE id = ANY(@vid)
               AND date_trunc('month', bucket::date) = date_trunc('month', now())
               ORDER BY  bucket DESC",
                new { vid = subjects });

                var groupMonth = EnergyMonthConsumption.GroupBy(p => new { p.Bucket.Year, p.Bucket.Month })
               .Select(g => new
               {
                   date_time = new DateTime(g.Key.Year, g.Key.Month, 1),
                   totalFirst = g.Sum(k => k.ValueFirst),
                   totalLast = g.Sum(k => k.ValueLast)
               }).ToList();

               var month = groupMonth.Select(p => p.totalLast - p.totalFirst).FirstOrDefault();

            if (EnergyConsumption.Count() == 0)
            {
                data =
                new GetAllTop5EnergyConsumptionsDto
                {
                    TotalWeek = 0,
                    TotalMonth = 0,
                    DataMachines = new List<DataMachine>(),
                };
            }
            else
            {
                data =
                new GetAllTop5EnergyConsumptionsDto
                {
                    TotalWeek = Math.Round((week),2),
                    TotalMonth = Math.Round((month),2),
                    DataMachines = Groups
                    .Select(val => new DataMachine
                    {
                        Label = subjectMachine.Where(k => val.id == k.Subject.Vid).Select(v => v.Machine.Name).FirstOrDefault(),
                        Value = val.totalLast - val.totalFirst,
                    }).OrderByDescending(v => v.Value).Take(5).ToList()
                };
            }
            return data;
        }

        public async Task<GetAllTop5MachineMaintenanceDto> GetAllTop5MachineMaintenance()
        {
 
            var maintenance = await _dapperReadDbConnection.QueryAsync<MaintenanceDto>
            (@"select mp.machine_id , m.""name"" as machine_name , count(mp.machine_id) as value from ""MaintenacePreventives"" mp left join ""Machines"" m on mp.machine_id = m.id  group by mp.machine_id, m.""name""");

            var maintenanceWeek = await _dapperReadDbConnection.QueryAsync<MaintenanceDto>
            (@"SELECT COUNT(mp.machine_id) AS value
            FROM ""MaintenacePreventives"" mp
            LEFT JOIN ""Machines"" m ON mp.machine_id = m.id
            WHERE DATE_PART('week', mp.created_at) = DATE_PART('week', CURRENT_DATE)");

            var maintenanceMonth = await _dapperReadDbConnection.QueryAsync<MaintenanceDto>
            (@"SELECT COUNT(mp.machine_id) AS value
            FROM ""MaintenacePreventives"" mp
            LEFT JOIN ""Machines"" m ON mp.machine_id = m.id
            WHERE DATE_PART('month', mp.created_at) = DATE_PART('month', CURRENT_DATE)");


            var data = new GetAllTop5MachineMaintenanceDto();

            if (maintenance.Count() == 0)
            {
                data = new GetAllTop5MachineMaintenanceDto
                {
                    DataMaintenance = maintenance.Select(x => new DataMaintenance
                    {
                        Label = x.MachineName,
                        Value = x.Value,
                    }).ToList()
                };
            }
            else
            {
                data = new GetAllTop5MachineMaintenanceDto
                {
                    TotalWeek = maintenanceWeek.Select(o => o.Value).First(),
                    TotalMonth = maintenanceMonth.Select(o => o.Value).First(),
                    DataMaintenance = maintenance.Select(x => new DataMaintenance
                    {
                        Label = x.MachineName,
                        Value = x.Value,
                    }).OrderByDescending(g => g.Value).Take(5).ToList()
                };
            }

            return data;  
    }
    }
}
