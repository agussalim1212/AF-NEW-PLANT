using FluentValidation;

namespace SkeletonApi.Application.Features.Subjects.Queries.GetSubjectWithPagination
{
    public class GetSubjectWithPaginationValidator : AbstractValidator<GetSubjectWithPaginationQuery>
    {
        public GetSubjectWithPaginationValidator()
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