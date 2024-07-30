using MediatR;
using SkeletonApi.Application.Features.ManagementUser;
using SkeletonApi.Shared;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Login.Commands
{
    public sealed record UserLoginRequest : IRequest<Result<TokenDto>>
    {
        [Required(ErrorMessage = "User name is required")]
        [JsonPropertyName("username")]
        public string? UserName { get; init; }
        [Required(ErrorMessage = "Password name is required")]
        [JsonPropertyName("password")]
        public string? Password { get; init; }
    }
}