namespace SkeletonApi.Shared.Interfaces
{
    public interface IResult<T>
    {
        List<string> Messages { get; set; }

        bool Status { get; set; }

        T Data { get; set; }
    }
}