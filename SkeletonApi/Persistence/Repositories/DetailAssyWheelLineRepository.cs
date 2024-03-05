using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelRearWithPagination;
using SkeletonApi.Application.Features.DetailMachine.AssyWheelLine.Queries.MachineInformationAssyWheelLine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;
using System.Collections.Immutable;
using System.Globalization;



namespace SkeletonApi.Persistence.Repositories
{
    public class DetailAssyWheelLineRepository : IDetailAssyWheelLineRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenRepository<SubjectHasMachine> _machineRepository;
        private readonly ApplicationDbContext _dbContext;

        public DetailAssyWheelLineRepository(IUnitOfWork unitOfWork, IDapperReadDbConnection dapperReadDbConnection, IGenRepository<SubjectHasMachine> machineRepository, ApplicationDbContext dbContext)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _unitOfWork = unitOfWork;
            _machineRepository = machineRepository;
            _dbContext = dbContext;

        }

        public async Task<GetAllMachineInformationAssyWheelLineDto> GetAllMachineInformationAsync(Guid machine_id)
        {
            var machine = await _dbContext.subjectHasMachines.Include(s => s.Machine).Include(s => s.Subject)
               .Where(m => (machine_id == m.MachineId && m.Subject.Vid.Contains("CYCLE-COUNT"))
               || (machine_id == m.MachineId && m.Subject.Vid.Contains("RUN-TIME"))
               || (machine_id == m.MachineId && m.Subject.Vid.Contains("RIM"))).ToListAsync();


            IEnumerable<string> vids = machine.Select(m => m.Subject.Vid).ToList();
            string machineName = machine.Select(x => x.Machine.Name).FirstOrDefault();
            string subjectName = machine.Select(x => x.Subject.Subjects).FirstOrDefault();

            var data = new GetAllMachineInformationAssyWheelLineDto();

            var categorys = await _dbContext.MachineInformation
             .Where(c => vids.Contains(c.Id))
             .GroupBy(c => c.Id)
             .Select(groups => new
             {
                 Id = groups.Key, // ID dari kelompok
                 LastRunTime = groups.Where(g => g.Id.Contains("RUN-TIME"))
                     .OrderByDescending(g => g.DateTime)
                     .FirstOrDefault(), // Get the last "Run-Time" element
                 LastCycleCount = groups.Where(g => g.Id.Contains("CYCLE-COUNT"))
                     .OrderByDescending(g => g.DateTime)
                     .FirstOrDefault(), // Get the last "Cycle-Count" element
                 LastKalibrasi = groups.Where(g => g.Id.Contains("RIM"))
                     .OrderByDescending(g => g.DateTime)
                     .FirstOrDefault(), // Get the last "rim-calibration" element
             })
             .ToListAsync();


            if (categorys.Count() == 0)
            {
                data = new GetAllMachineInformationAssyWheelLineDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,

                };

            }
            else
            {


                data = new GetAllMachineInformationAssyWheelLineDto
                {
                    MachineName = machineName,
                    SubjectName = subjectName,
                    DateTime = DateTime.Now,
                    ValueRunning = categorys.Select(c => Convert.ToDecimal(c.LastRunTime?.Value)).Skip(1).FirstOrDefault(),
                    CycleCount = categorys.Select(x => Convert.ToDecimal(x.LastCycleCount?.Value)).FirstOrDefault(),
                    LastTimeCalibration = categorys.Select(n => n.LastKalibrasi?.Value).Skip(2).FirstOrDefault(),

                };
            }
            return data;
        }

        public async Task<List<GetListWheelRearDto>> GetListWheelRearQuality(Guid machineId, string typesWheel, string type, DateTime start, DateTime end)
        {
            var machine = _machineRepository.FindByCondition(o => o.MachineId == machineId).Include(p => p.Machine).Include(p => p.Subject).ToList();
           
            var status = machine.Where(m => m.Subject.Vid.Contains("FI-STATUS-PRODUKSI")).FirstOrDefault();
            var horizontal = machine.Where(m => m.Subject.Vid.Contains("FI-DIAL-HORIZONTAL")).FirstOrDefault();
            var vertikal = machine.Where(m => m.Subject.Vid.Contains("FI-DIAL-VERTIKAL")).FirstOrDefault();
            var diskBrake = machine.Where(m => m.Subject.Vid.Contains("FI-DISK-BRAKE")).FirstOrDefault();

          


            List<GetListWheelRearDto> dt = new List<GetListWheelRearDto>();
            var data = new GetListWheelRearDto();

            throw new NotImplementedException();
        }

        public async Task<List<GetVid>> GetVidsAsync(Guid machineId, string category)
        {
            var machine = _machineRepository.FindByCondition(o => o.MachineId == machineId).Include(p => p.Machine).Include(p => p.Subject).ToList();
            List<GetVid> dt = new List<GetVid>();
                var status = machine.Where(m => m.Subject.Vid.Contains("FI-STATUS-PRODUKSI")).FirstOrDefault();
            //if(category == "final_inspection")
            //{
            //    var status = machine.Where(m => m.Subject.Vid.Contains("FI-STATUS-PRODUKSI")).FirstOrDefault();
            //    var horizontal = machine.Where(m => m.Subject.Vid.Contains("FI-DIAL-HORIZONTAL")).FirstOrDefault();
            //    var vertikal = machine.Where(m => m.Subject.Vid.Contains("FI-DIAL-VERTIKAL")).FirstOrDefault();
            //    var diskBrake = machine.Where(m => m.Subject.Vid.Contains("FI-DISK-BRAKE")).FirstOrDefault();


            //}



            throw new NotImplementedException();
        }
    }
}
    
