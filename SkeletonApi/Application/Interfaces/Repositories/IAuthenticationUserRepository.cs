using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Application.Features.Users.Login.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IAuthenticationUserRepository
    {
        //Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);

        //Task<bool> ValidateUser(UserLoginRequest userForAuth);

        //Task<TokenDto> CreateToken(bool populateExp);
    }
}
