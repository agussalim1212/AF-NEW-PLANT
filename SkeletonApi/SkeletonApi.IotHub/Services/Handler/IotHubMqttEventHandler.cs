using SkeletonApi.IotHub.Model;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SkeletonApi.IotHub.Services.Handler
{
    public class IotHubMqttEventHandler : IIoTHubEventHandler<MqttRawDataEncapsulation>,IDisposable
    {
        private readonly BehaviorSubject<MqttRawDataEncapsulation> _subject;
        private readonly Dictionary<string, IDisposable> _subscribers;

        public IotHubMqttEventHandler()
        {
            _subject = new BehaviorSubject<MqttRawDataEncapsulation>(new MqttRawDataEncapsulation(null,new MqttRawData()));
            _subscribers = new Dictionary<string, IDisposable>();
        }

        public void Dispatch(MqttRawDataEncapsulation eventMessage)
        {
            _subject.OnNext(eventMessage);
        }

        public void Subscribe(string subscriberName, Action<MqttRawDataEncapsulation> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Subscribe(action));
            }
        }

        public void Subscribe(string subscriberName, Func<MqttRawDataEncapsulation, bool> predicate, Action<MqttRawDataEncapsulation> action)
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
