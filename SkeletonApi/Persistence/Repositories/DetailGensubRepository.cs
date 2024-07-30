using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Configuration;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Persistence.Repositories
{
    public class DetailGensubRepository : IDetailGensubRespository
    {
        private readonly IDapperReadDbConnection _dapperReadDbConnection;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Machine> _repositoryMachine;

        public DetailGensubRepository(IDapperReadDbConnection dapperReadDbConnection, IUnitOfWork unitOfWork, IGenericRepository<Machine> repositoryMachine)
        {
            _dapperReadDbConnection = dapperReadDbConnection;
            _unitOfWork = unitOfWork;
            _repositoryMachine = repositoryMachine;
        }
    }
}