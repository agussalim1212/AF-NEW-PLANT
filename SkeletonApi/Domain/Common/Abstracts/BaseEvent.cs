using MediatR;

namespace SkeletonApi.Domain.Common.Abstracts
{
    public abstract class BaseEvent : INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.Now;
    }
}