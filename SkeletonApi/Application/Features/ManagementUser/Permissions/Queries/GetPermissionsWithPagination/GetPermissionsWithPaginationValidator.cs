using FluentValidation;

namespace SkeletonApi.Application.Features.ManagementUser.Permissions.Queries.GetPermissionsWithPagination
{
    public class GetPermissionsWithPaginationValidator : AbstractValidator<GetPermissionsWithPaginationQuery>
    {
        public GetPermissionsWithPaginationValidator()
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