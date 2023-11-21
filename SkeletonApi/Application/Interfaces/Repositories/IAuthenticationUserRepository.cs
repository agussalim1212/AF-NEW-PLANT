using Microsoft.AspNetCore.Identity;
using SkeletonApi.Application.Features.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IAuthenticationUserRepository
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);

        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);

        Task<TokenDto> CreateToken(bool populateExp);
    }
}
