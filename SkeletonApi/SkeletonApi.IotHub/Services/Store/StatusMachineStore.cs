using AutoMapper;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.IotHub.DTOs;

namespace SkeletonApi.IotHub.Services.Store
{
    public class StatusMachineStore
    {
        private IEnumerable<MachineStatusDto> _Machine { get; set; }

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public StatusMachineStore(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
            _Machine = new List<MachineStatusDto>();
            this.Dispatch();
        }

        public Task Dispatch()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scoped = scope.ServiceProvider.GetRequiredService<IStatusMachineRepository>();
                var machines = scoped.GetAllMachinesAsync().Result;

                //MainSubject traceAbilityResult = scoped.GetMainSubjectAsync("Process-Part").Result;
                //_traceabilityResultSubjectStore = _mapper.Map<MainSubjectStoreDto>(traceAbilityResult);
                _Machine = _mapper.Map<IEnumerable<MachineStatusDto>>(machines);
            }
            return Task.CompletedTask;
        }

        public IEnumerable<MachineStatusDto> GetAllMachine()
        {
            return _Machine;
        }
    }
}
