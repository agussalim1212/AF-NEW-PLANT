using SkeletonApi.IotHub.Model;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SkeletonApi.IotHub.Services.Handler
{
    public class IotHubNotificationEventHandler : IIoTHubEventHandler<IEnumerable<NotificationModel>>, IDisposable
    {
        private readonly BehaviorSubject<IEnumerable<NotificationModel>> _subject;
        private readonly Dictionary<string, IDisposable> _subscribers;

        public IotHubNotificationEventHandler()
        {
            _subject = new BehaviorSubject<IEnumerable<NotificationModel>>(new List<NotificationModel>());
            _subscribers = new Dictionary<string, IDisposable>();
        }

        public void Dispatch(IEnumerable<NotificationModel> eventMessage)
        {
            _subject.OnNext(eventMessage);
        }

        public void Subscribe(string subscriberName, Action<IEnumerable<NotificationModel>> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Subscribe(action));
            }
        }
        public IObservable<IEnumerable<NotificationModel>> Observe()
        {
            return _subject;
        }
        public void Subscribe(string subscriberName, Func<IEnumerable<NotificationModel>, bool> predicate, Action<IEnumerable<NotificationModel>> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Where(predicate).Subscribe(action));
            }
        }

        public void Dispose()
        {
            if (_subject != null)
            {
                _subject.Dispose();
            }

            foreach (var subscriber in _subscribers)
            {
                subscriber.Value.Dispose();
            }
        }
    }


}
