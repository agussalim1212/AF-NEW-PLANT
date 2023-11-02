using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.CategoryMachine.Commands.DeleteCategoryHasMachine;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.SubjectHasMachines.Commands.DeleteSubjectHasMachine
{
    public record DeleteSubjectHasMachinesCommand : IRequest<Result<Guid>>, IMapFrom<SubjectHasMachine>
    {
        public Guid Id { get; set; }

        public DeleteSubjectHasMachinesCommand()
        {

        }
        public DeleteSubjectHasMachinesCommand(Guid id)
        {
            Id = id;
        }
    }

    internal class DeleteSubjectHasMachinesHandler : IRequestHandler<DeleteSubjectHasMachinesCommand, Result<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSubjectHasMachinesHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteSubjectHasMachinesCommand request, CancellationToken cancellationToken)
        {
            var subjectMachines = await _unitOfWork.Repo<SubjectHasMachine>().Entities.Where(x => request.Id == x.MachineId).ToListAsync();
            Console.WriteLine(subjectMachines);

            if (subjectMachines.Count != 0)
            {
                foreach (var sM in subjectMachines)
                {
                    await _unitOfWork.Repo<SubjectHasMachine>().DeleteAsync(sM);
                    sM.AddDomainEvent(new SubjectHasMachineDeleteEvent(sM));
                    await _unitOfWork.Save(cancellationToken);
                }
                return await Result<Guid>.SuccessAsync(request.Id, "Subject Has Machines Deleted");
            }

            return await Result<Guid>.FailureAsync("Subject Has Machines not found");
        }


    }
}
