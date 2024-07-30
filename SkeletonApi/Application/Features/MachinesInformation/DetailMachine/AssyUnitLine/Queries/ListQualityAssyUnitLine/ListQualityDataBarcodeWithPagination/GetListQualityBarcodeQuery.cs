using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityDataBarcodeWithPagination
{
    public class GetListQualityBarcodeQuery : IRequest<PaginatedResult<GetListQualityBarcodeDto>>
    {
        public Guid MachineId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Type { get; set; }
        public string? SearchTerm { get; set; }

        public GetListQualityBarcodeQuery()
        {
        }

        public GetListQualityBarcodeQuery(Guid machineId, int pageNumber, int pageSize, string type, string searchTerm)
        {
            MachineId = machineId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Type = type;
            SearchTerm = searchTerm;
        }
    }

    internal class GetListQualityAutoQGateQueryHandler : IRequestHandler<GetListQualityBarcodeQuery, PaginatedResult<GetListQualityBarcodeDto>>
    {
        private readonly IDetailMachineRepository _detailMachineRepository;
        private readonly IDefaultRepository _defaultRepository;


        public GetListQualityAutoQGateQueryHandler(IDetailMachineRepository detailMachineRepository, IDefaultRepository defaultRepository)
        {
            _detailMachineRepository = detailMachineRepository;
            _defaultRepository = defaultRepository;
        }

        public async Task<PaginatedResult<GetListQualityBarcodeDto>> Handle(GetListQualityBarcodeQuery query, CancellationToken cancellationToken)
        {
            //mencari vid subject berdasarkan id machine dan vid yang mengandung data barcode
            var vid = await _detailMachineRepository.GetSubjectAsync(query.MachineId, "DATA-BARCODE");

            //mengambil data yang memiliki vid di atas
            var data = await _detailMachineRepository.GetListQualitYBarcode(vid.Vid);
            return await data.ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}