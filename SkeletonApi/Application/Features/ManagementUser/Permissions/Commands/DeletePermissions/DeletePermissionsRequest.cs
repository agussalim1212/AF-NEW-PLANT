using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.DeletePermissions
{
    public class DeletePermissionsRequest : IRequest<Result<string>>, IMapFrom<Permission>
    {
        public string Id { get; set; }

        public DeletePermissionsRequest(string id)
        {
            Id = id;
        }

        public DeletePermissionsRequest()
        { }
    }
}