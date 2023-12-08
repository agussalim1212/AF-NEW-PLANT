﻿using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ManagementUser.Users.Queries.GetUserWithPagination
{
    public class GetUserWithPaginationDto : IMapFrom<GetUserWithPaginationDto>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("username")]
        public string? UserName { get; set; }
        [JsonPropertyName("password")]
        public string? PasswordHash { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("role")]
        public List<Roles> Roles { get; set; }
        [JsonPropertyName("last_created")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class Roles
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
