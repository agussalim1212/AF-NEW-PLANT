using SkeletonApi.Application.Common.Mappings;


namespace SkeletonApi.Application.Features.Settings.Queries.GetSubjectByMachineId
{
    public record GetSubjectByMachineIdDto : IMapFrom<GetSubjectByMachineIdDto>
    {
        public string Subject { get; set; }

    }
}
