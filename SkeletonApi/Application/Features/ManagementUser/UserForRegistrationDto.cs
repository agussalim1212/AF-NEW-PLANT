using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Users
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
