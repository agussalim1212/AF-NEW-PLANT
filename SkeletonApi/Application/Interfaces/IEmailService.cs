using SkeletonApi.Application.DTOs.Email;

namespace SkeletonApi.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto request);
    }
}