namespace SkeletonApi.Application.Common.Exceptions
{
    public sealed class GeneralBadRequest : BadRequestException
    {
        public GeneralBadRequest(string message) : base(message)
        {
        }
    }
}
