using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Accounts
{
    public record PasswordDto
    {
        [JsonPropertyName("current_password")]
        public string CurrentPassword { get; set; }
        [JsonPropertyName("new_password")]
        public string NewPassword { get; set; }
        [JsonPropertyName("repeat_password")]
        public string RepeatPassword { get; set; }
    }
    public sealed record CreatePasswordResponseDto : PasswordDto { }
}
