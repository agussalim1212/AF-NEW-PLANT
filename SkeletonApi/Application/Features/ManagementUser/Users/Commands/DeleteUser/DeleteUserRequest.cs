using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;


namespace SkeletonApi.Application.Features.ManagementUser.Users.Commands.DeleteUser
{
    public record DeleteUserRequest : IRequest<Result<string>>, IMapFrom<User>
    {
        public string Id { get; set; }
        public DeleteUserRequest(string id)
        {
            Id = id;
        }

        public DeleteUserRequest() { }
    }
}
