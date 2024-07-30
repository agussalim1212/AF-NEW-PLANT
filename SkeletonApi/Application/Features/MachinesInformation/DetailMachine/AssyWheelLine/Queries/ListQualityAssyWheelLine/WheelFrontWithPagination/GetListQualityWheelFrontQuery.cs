using MediatR;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Filtering;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.MachinesInformation.DetailMachine.AssyWheelLine.Queries.ListQualityAssyWheelLine.WheelFrontWithPagination
{
    public record GetListQualityWheelFrontQuery : IRequest<PaginatedResult<GetListWheelFrontDto>>
    {
        public Guid machine_id { get; set; }
        public string type_wheel { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public string search_term { get; set; }
        public string type { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }

        public GetListQualityWheelFrontQuery() { }

        public GetListQualityWheelFrontQuery(DateTime Start, DateTime End, string typesWheel, string searchTerm, Guid machineId, int pageNumber, int pageSize, string Type)
        {
            machine_id = machineId;
            page_number = pageNumber;
            page_size = pageSize;
            search_term = searchTerm;
            type_wheel = typesWheel;
            type = Type;
            start = Start;
            end = End;
        }

        internal class GetListQualityWheelFrontQueryHandler : IRequestHandler<GetListQualityWheelFrontQuery, PaginatedResult<GetListWheelFrontDto>>
        {
            private readonly IDetailMachineRepository _detailMachineRepository;
            private readonly IDayRepository _dayRepository;
            private readonly IDefaultRepository _defaultRepository;

            public GetListQualityWheelFrontQueryHandler(IDetailMachineRepository detailMachineRepository, IDayRepository dayRepository, IDefaultRepository defaultRepository)
            {
                _detailMachineRepository = detailMachineRepository;
                _dayRepository = dayRepository;
                _defaultRepository = defaultRepository;
            }

            public async Task<PaginatedResult<GetListWheelFrontDto>> Handle(GetListQualityWheelFrontQuery query, CancellationToken cancellationToken)
            {
                List<GetListWheelFrontDto> data = new List<GetListWheelFrontDto>();

                //Press Bearing
                var vidPbqStatus = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-STATUS-PRODUKSI");
                var vidPbqDistance = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-HORIZONTAL");
                var vidPbsPressLoad = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-VERTIKAL");

                //Final Inspection
                var vidStatus = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-STATUS-PRODUKSI");
                var vidHorizontal = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-HORIZONTAL");
                var vidVertikal = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-VERTIKAL");
                var vidDiskBrake = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "FI-DIAL-DISK-BRAKE");

                //Tire Inflation
                var vidTire = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "TIQ-TIRE-PRASSURE");

                //Disk Brake
                var vidDisk = await _detailMachineRepository.GetSubjectAsync(query.machine_id, "DBQ-TORQUE1");

                if (query.type == "day" || query.type == "week" || query.type == "month" || query.type == "year")
                {
                    if (query.type_wheel == "final_inspection")
                    {
                        var day = await _dayRepository.GetListQualityWheelFrontFinalInspectionDay(vidStatus.Vid, vidHorizontal.Vid, vidVertikal.Vid, vidDiskBrake.Vid, query.machine_id, query.start, query.end);
                        var paged = day.Where(c => query.search_term == null
                         || query.search_term.ToLower() == c.Status.ToLower()
                         || query.search_term.ToLower() == c.DataDialHorizontal.ToLower()
                         || query.search_term.ToLower() == c.DataDialVertical.ToLower()
                         || query.search_term.ToLower() == c.DiskBrake.ToLower()).ToList();
                        data.AddRange(paged);
                    }
                    else if (query.type_wheel == "tire_inflation")
                    {
                    }
                    else if (query.type_wheel == "disk_brake")
                    {
                    }
                    else
                    {
                    }
                }
                else
                {
                    if (query.type_wheel == "final_inspection")
                    {
                    }
                    else if (query.type_wheel == "tire_inflation")
                    {
                        var defaultData = await _defaultRepository.GetListQualityAssyWheelFrontTireInflationDefault(vidTire.Vid);
                        data.AddRange(defaultData);
                    }
                    else if (query.type_wheel == "disk_brake")
                    {
                        var defaultData = await _defaultRepository.GetListQualityAssyWheelFrontDiskBrake(vidDisk.Vid);
                        data.AddRange(defaultData);
                    }
                    else
                    {
                    }
                }

                return await data.ToPaginatedListAsync(query.page_number, query.page_size, cancellationToken);
            }
        }
    }
}