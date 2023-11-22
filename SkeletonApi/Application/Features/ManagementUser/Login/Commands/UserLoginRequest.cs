using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.Users.Login.Commands
{
    public sealed record UserLoginRequest 
    {
        [Required(ErrorMessage = "User name is required")]
        [JsonPropertyName("username")]
        public string? UserName { get; init; }
        [Required(ErrorMessage = "Password name is required")]
        [JsonPropertyName("password")]
        public string? Password { get; init; }
    }
}
