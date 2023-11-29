using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities.Exceptions
{
    public sealed class RefreshTokenBadRequest : BadRequestException
    {
        public RefreshTokenBadRequest()
        : base("Invalid client request. The tokenDto has some invalid values.")
        {
        }
    }
}
