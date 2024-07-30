using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.DTOs.DetailMachine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Configuration;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;

namespace SkeletonApi.Persistence.Repositories
{
    public class DetailAssyWheelLineRepository : IDetailAssyWheelLineRepository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenRepository<SubjectHasMachine> _machineRepository;
        private readonly IGenericRepository<Machine> _repositoryMachine;
        private readonly ApplicationDbContext _dbContext;

        public DetailAssyWheelLineRepository(IUnitOfWork unitOfWork, IDapperReadDbConnection dapperReadDbConnection, IGenericRepository<Machine> repositoryMachine, IGenRepository<SubjectHasMachine> machineRepository, ApplicationDbContext dbContext)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _unitOfWork = unitOfWork;
            _machineRepository = machineRepository;
            _dbContext = dbContext;
            _repositoryMachine = repositoryMachine;
        }

        public async Task<GetVidSubjectDto> GetVidsAsync(Guid machineId, string vid)
        {
            var getVid = _machineRepository.FindByCondition(o => o.MachineId == machineId).Include(p => p.Subject)
                          .Where(p => p.Subject.Vid.Contains(vid)).Select(o => new GetVidSubjectDto { Vid = o.Subject.Vid }).FirstOrDefault();
            return getVid;
        }
    }
}