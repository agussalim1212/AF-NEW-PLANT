using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities.Exceptions
{
    public abstract class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
        : base(message)
        { }
    }
}
