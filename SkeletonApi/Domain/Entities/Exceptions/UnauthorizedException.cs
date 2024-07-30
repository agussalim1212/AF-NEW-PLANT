namespace SkeletonApi.Domain.Entities.Exceptions
{
    public abstract class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
        : base(message)
        { }
    }
}