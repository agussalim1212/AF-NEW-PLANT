namespace SkeletonApi.IotHub.Services.Handler
{
    public interface IIoTHubEventHandler<T>
    {
        void Dispatch(T val);

        //IObservable<T> Observe();
        void Subscribe(string subscriberName, Action<T> action);

        void Subscribe(string subscriberName, Func<T, bool> predicate, Action<T> action);
    }
}