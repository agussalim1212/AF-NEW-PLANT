using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Users
{
    public record UserForAuthenticationDto 
    {
        [Required(ErrorMessage = "User name is required")]
        [JsonPropertyName("username")]
        public string? UserName { get; init; }
        [Required(ErrorMessage = "Password name is required")]
        [JsonPropertyName("password")]
        public string? Password { get; init; }
    }
}
