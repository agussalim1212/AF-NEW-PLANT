using AutoMapper;
using MediatR;
using SkeletonApi.Application.Features.Subjects.Commands.CreateSubject;
using SkeletonApi.Application.Features.Subjects;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using Microsoft.AspNetCore.Identity;

namespace SkeletonApi.Application.Features.Users.CreateUser
{
    internal class CreateUserCommandHandler : IRequestHandler<CreateUserRequest, Result<CreateUserResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
       
        public CreateUserCommandHandler( IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
           
        }

        public async Task<Result<CreateUserResponseDto>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var UserCreated = _mapper.Map<User>(request);
            //var validateData = await _subjectRepository.ValidateData(Subjects);

            //if (validateData != true)
            //{
            //    return await Result<CreateSubjectResponseDto>.FailureAsync("Data already exist");
            //}

            UserCreated.CreatedAt = DateTime.UtcNow;
            UserCreated.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Data<User>().AddAsync(UserCreated);
           // UserCreated.AddDomainEvent(new SubjectCreatedEvent(Subjects));
            await _unitOfWork.Save(cancellationToken);
            var userResponse = _mapper.Map<CreateUserResponseDto>(UserCreated);

            return await Result<CreateUserResponseDto>.SuccessAsync(userResponse, "User created.");

        }
    }
}
