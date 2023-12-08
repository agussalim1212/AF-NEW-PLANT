using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Settings.Queries.GetSettingWithPagination
{
    public record GetSettingWithPaginationQuery : IRequest<PaginatedResult<GetSettingWithPaginationDto>>
    {
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }


        public GetSettingWithPaginationQuery() { }

        public GetSettingWithPaginationQuery(string searchTerm, int pageNumber, int pageSize)
        {
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;

        }
    }
    internal class GetSettingWithPaginationQueryHandler : IRequestHandler<GetSettingWithPaginationQuery, PaginatedResult<GetSettingWithPaginationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSettingWithPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetSettingWithPaginationDto>> Handle(GetSettingWithPaginationQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<Setting>().Entities.Where(o => query.search_term == null 
            || query.search_term.ToLower() == o.MachineName.ToLower()
            || query.search_term.ToLower() == o.SubjectName.ToLower())
                   .OrderByDescending(x => x.UpdatedAt)
                   .ProjectTo<GetSettingWithPaginationDto>(_mapper.ConfigurationProvider)
                   .ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
           
        }
    }
}
