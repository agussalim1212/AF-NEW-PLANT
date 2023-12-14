using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Notification.Queries.GetListNotif
{
    public record class GetListNotifQuery : IRequest<Result<List<GetListNotifDto>>>;

    internal class GetListNotifQueryHandler : IRequestHandler<GetListNotifQuery, Result<List<GetListNotifDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListNotifQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetListNotifDto>>> Handle(GetListNotifQuery getList, CancellationToken cancellationToken)
        {
            var sql = await _unitOfWork.Repository<Notifications>().Entities
                .Where(x => x.Status == false)
                .OrderByDescending(x => x.DateTime)
                .Take(3)
                .Select(x => new GetListNotifDto
                {
                    Id = x.Id,
                    MachineName = x.MachineName,
                    DateTime = x.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Message = x.Message,
                    Status = x.Status,
                }).ToListAsync(cancellationToken);

            return await Result<List<GetListNotifDto>>.SuccessAsync(sql, "Successfully fetch data");
        }
    }
}
