using FluentValidation;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Queries.DownloadList
{
    public class DownloadListMaintPrevValidator : AbstractValidator<DownloadListMaintPrevQuery>
    {
        public DownloadListMaintPrevValidator()
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