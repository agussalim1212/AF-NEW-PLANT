using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser
{
    public record UserForRegistrationDto
    {
        [Required(ErrorMessage = "Username is required")]
        [JsonPropertyName("username")]
        public string? UserName { get; init; }
        [JsonPropertyName("email")]
        public string? Email { get; init; }
        [Required(ErrorMessage = "Password is required")]
        [JsonPropertyName("password")]
        public string? Password { get; init; }
        [JsonPropertyName("role")]
        public ICollection<string>? Roles { get; init; }
    }
    public sealed record CreateUserResponseDto : UserForRegistrationDto { }
}