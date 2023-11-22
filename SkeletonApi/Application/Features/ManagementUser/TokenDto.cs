using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Users
{
    public record TokenDto(string AccessToken, string RefreshToken);
  
}
