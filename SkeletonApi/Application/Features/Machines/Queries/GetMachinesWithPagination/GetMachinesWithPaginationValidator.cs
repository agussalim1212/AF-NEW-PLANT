﻿using FluentValidation;
using SkeletonApi.Application.Features.Machines.Queries.GetAllMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Machines.Queries.GetMachinesWithPagination
{
    public class GetMachinesWithPaginationValidator : AbstractValidator<GetMachinesWithPaginationQuery>
    {
        public GetMachinesWithPaginationValidator() 
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
