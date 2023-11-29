﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.AssyUnitLine.Queries.ListQualityAssyUnitLine.ListQualityOilBrake
{
    public class GetListQualityOilBrakeValidator : AbstractValidator<GetListQualityOilBrakeQuery>
    {
        public GetListQualityOilBrakeValidator() 
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