using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;

namespace SkeletonApi.Application.Features.Accounts.Password.Command.ChangesPassword
{
    public record UpdatePasswordCommand : IRequest<Result<string>>
    {
        //dari user yang sedang login
        public string Username { get; set; }
        [JsonPropertyName("current_password")]
        //password saat ini
        public string CurrentPassword { get; set; }
        [JsonPropertyName("new_password")]
        //password baru
        public string NewPassword { get; set; }
        [JsonPropertyName("repeat_password")]
        //ulangi password baru
        public string RepeatPassword { get; set; }
    }

    internal class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var cekUser = _unitOfWork.Data<User>().FindByCondition(o => o.UserName == command.Username).FirstOrDefault();
            if (cekUser != null)
            {
                _userManager.ChangePasswordAsync(cekUser, command.CurrentPassword, command.RepeatPassword);
                return await Result<string>.SuccessAsync(cekUser.UserName, "Password Updated.");
            }
            else
            {
                //jika username tidak ditemukan
                return await Result<string>.FailureAsync(cekUser.UserName, "User Not Found.");
            }
        }
    }
}