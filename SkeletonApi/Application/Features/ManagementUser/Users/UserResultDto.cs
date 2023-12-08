using SkeletonApi.Application.Features.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.ManagementUser.Users
{
    public record UserResultDto<T>
    {
        public T Result { get; set; }
        public UserForRegistrationDto User { get; set; }
    }
}
