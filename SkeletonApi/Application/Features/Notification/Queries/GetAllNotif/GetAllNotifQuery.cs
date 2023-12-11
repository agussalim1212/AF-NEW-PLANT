using AutoMapper;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Notification.Queries.GetAllNotif
{
    public record class GetAllNotifQuery : IRequest<PaginatedResult<GetAllNotifDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }

        public GetAllNotifQuery()
        {
            
        }

        public GetAllNotifQuery(int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
        }
    }

    internal class GetAllNotifQueryHandler : IRequestHandler<GetAllNotifQuery, PaginatedResult<GetAllNotifDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllNotifQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetAllNotifDto>> Handle(GetAllNotifQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Notifications>().Entities
                .OrderByDescending(x => x.DateTime)
                .Select(x => new GetAllNotifDto
                {
                    Id = x.Id,
                    MachineName = x.MachineName,
                    DateTime = x.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Message = x.Message,
                    Status = x.Status,
                }).ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
        }
    }
}
