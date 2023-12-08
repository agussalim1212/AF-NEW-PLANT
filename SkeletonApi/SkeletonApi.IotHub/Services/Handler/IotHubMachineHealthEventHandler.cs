using SkeletonApi.IotHub.Model;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using YamlDotNet.Core.Tokens;

namespace SkeletonApi.IotHub.Services.Handler
{
    public class IotHubMachineHealthEventHandler : IIoTHubEventHandler<IEnumerable<MachineHealthModel>>, IDisposable
    {
        private readonly BehaviorSubject<IEnumerable<MachineHealthModel>> _subject;
        private readonly Dictionary<string, IDisposable> _subscribers;

        public IotHubMachineHealthEventHandler()
        {
            _subject = new BehaviorSubject<IEnumerable<MachineHealthModel>>(new List<MachineHealthModel>());
            _subscribers = new Dictionary<string, IDisposable>();
        }

        public void Dispatch(IEnumerable<MachineHealthModel> eventMessage)
        {
            _subject.OnNext(eventMessage);
        }

        public void Subscribe(string subscriberName, Action<IEnumerable<MachineHealthModel>> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Subscribe(action));
            }
        }
        public IObservable<IEnumerable<MachineHealthModel>> Observe()
        {
            return _subject;
        }
        public void Subscribe(string subscriberName, Func<IEnumerable<MachineHealthModel>, bool> predicate, Action<IEnumerable<MachineHealthModel>> action)
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