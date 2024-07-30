using MediatR;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.UpdateUser
{
    public class UpdateUserRequest : IRequest<Result<User>>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string PasswordHash { get; set; }

        [JsonPropertyName("role")]
        public ICollection<string>? Roles { get; set; }
    }
}