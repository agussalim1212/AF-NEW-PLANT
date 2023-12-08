using MediatR;
using Microsoft.AspNetCore.Http;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.CreateAccount
{
    public sealed record CreateAccountRequest : IRequest<Result<CreateAccountResponseDto>>
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        public IFormFile img_path { get; set; }
    }
}
