using Microsoft.AspNetCore.Http;


namespace SkeletonApi.Application.Features.Accounts
{
    public record AccountDto
    {
        public IFormFile img_path { get; set; }
    }
    public sealed record CreateAccountResponseDto : AccountDto { }
}
