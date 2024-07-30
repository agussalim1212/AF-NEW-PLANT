using FluentValidation;
using SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityAssyUnitLineWithPagination;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityNutRunnerSteeringStemWithPagination
{
    public class GetListQualityNutRunnerSteeringStemValidator : AbstractValidator<GetListQualityNutRunnerSteeringStemQuery>
    {
        public GetListQualityNutRunnerSteeringStemValidator()
        {
            RuleFor(x => x.page_number)
                 .GreaterThanOrEqualTo(1)
                 .WithMessage("PageNumber at least greater than or equal to 1.");

            RuleFor(x => x.page_size)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageSize at least greater than or equal to 1.");
        }
    }
}