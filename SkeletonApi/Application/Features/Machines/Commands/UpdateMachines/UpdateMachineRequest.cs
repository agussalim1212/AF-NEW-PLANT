﻿using MediatR;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Machines.Commands.UpdateMachines
{
    public record UpdateMachineRequest : IRequest<Result<Machine>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
