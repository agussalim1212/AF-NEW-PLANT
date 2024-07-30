using SkeletonApi.Application.Interfaces;

namespace SkeletonApi.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}