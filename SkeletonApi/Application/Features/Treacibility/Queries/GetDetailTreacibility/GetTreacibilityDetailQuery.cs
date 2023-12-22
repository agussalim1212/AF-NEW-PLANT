using MediatR;
using SkeletonApi.Application.Features.Subjects.Queries.GetAllSubject;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Treacibility.Queries.GetDetailTreacibility
{
    public record GetTreacibilityDetailQuery : IRequest<Result<List<GetTreacibilityDetailDto>>>;
   
    
}
