using AutoMapper;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.IotHub.DTOs;

namespace SkeletonApi.IotHub.Services.Store
{
    public class NotificationStore
    {
        private IEnumerable<NotificationDto> _Setting { get; set; }

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public NotificationStore(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
            _Setting = new List<NotificationDto>();
            this.Dispatch();
        }

        public Task Dispatch()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scoped = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var machines = scoped.GetAllSettingAsync().Result;
        
                _Setting = _mapper.Map<IEnumerable<NotificationDto>>(machines);
            }
            return Task.CompletedTask;
        }

        public IEnumerable<NotificationDto> GetAllSetting()
        {
            return _Setting;
        }
    }
}
