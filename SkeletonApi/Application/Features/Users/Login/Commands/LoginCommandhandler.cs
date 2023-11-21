using AutoMapper;
using MediatR;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Shared;

namespace SkeletonApi.Application.Features.Users.Login.Commands
{
    //internal class LoginCommandHandler : IRequestHandler<UserLoginRequest, Result<LoginResponseDto>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IAuthenticationUserRepository _userRepository;
    //    private readonly IMapper _mapper;

    //    public LoginCommandHandler(IUnitOfWork unitOfWork, IAuthenticationUserRepository userRepository, IMapper mapper)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _userRepository = userRepository;
    //        _mapper = mapper;
    //    }

    //    public async Task<Result<LoginResponseDto>> Handle(UserLoginRequest request, CancellationToken cancellationToken)
    //    {
    //        await _userRepository.ValidateUser(request.);
    //    }


    //}
}
