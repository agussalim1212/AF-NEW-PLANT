using AutoMapper;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.IotHub.DTOs;

namespace SkeletonApi.IotHub.Services.Store
{
    public class SubjectStore
    {
        private IEnumerable<SubjectDto> _subject { get; set; }

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public SubjectStore(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
            _ = new List<SubjectDto>();
            this.Dispatch();
        }

        public Task Dispatch()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scoped = scope.ServiceProvider.GetRequiredService<ISubjectRepository>();
                var machines = scoped.GetAllSubjectAsync().Result;

                _subject = _mapper.Map<IEnumerable<SubjectDto>>(machines);
            }
            return Task.CompletedTask;
        }

        public IEnumerable<SubjectDto> GetAllSubject()
        {
            return _subject;
        }
    }
}
