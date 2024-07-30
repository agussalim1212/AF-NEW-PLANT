namespace SkeletonApi.Application.Features.ManagementUser.Users
{
    public record UserResultDto<T>
    {
        public T Result { get; set; }
        public UserForRegistrationDto User { get; set; }
    }
}