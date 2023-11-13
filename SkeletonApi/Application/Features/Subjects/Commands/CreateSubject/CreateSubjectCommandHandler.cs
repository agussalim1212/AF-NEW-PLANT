using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Subjects.Commands.CreateSubject
{

    internal class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectRequest, Result<CreateSubjectResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public CreateSubjectCommandHandler(IUnitOfWork unitOfWork, ISubjectRepository subjectRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<Result<CreateSubjectResponseDto>> Handle(CreateSubjectRequest request, CancellationToken cancellationToken)
        {
            var Subjects = _mapper.Map<Subject>(request);
            //var validateData = await _subjectRepository.ValidateData(Subjects);

            //if (validateData != true)
            //{
            //    return await Result<CreateSubjectResponseDto>.FailureAsync("Data already exist");
            //}

            await _unitOfWork.Repository<Subject>().AddAsync(Subjects);
            Subjects.AddDomainEvent(new SubjectCreatedEvent(Subjects));
            await _unitOfWork.Save(cancellationToken);
            var subjectMachineResponse = _mapper.Map<CreateSubjectResponseDto>(Subjects);
            return await Result<CreateSubjectResponseDto>.SuccessAsync(subjectMachineResponse, "Subject created.");
        }
    }
}