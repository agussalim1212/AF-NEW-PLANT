namespace SkeletonApi.Domain.Entities.Exceptions
{
    public sealed class FailedAuthenticationException : UnauthorizedException
    {
        public FailedAuthenticationException(string message)
        : base(message)
        {
        }
    }
}