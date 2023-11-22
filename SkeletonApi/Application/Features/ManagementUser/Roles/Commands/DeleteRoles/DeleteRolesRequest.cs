using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Roles.Commands.DeleteRoles
{
    public class DeleteRolesRequest : IRequest<Result<string>>, IMapFrom<Role>
    {
        public string Id { get; set; }
        public DeleteRolesRequest(string id) 
        {
            Id = id;
        }
        public DeleteRolesRequest() { }
    }
}
