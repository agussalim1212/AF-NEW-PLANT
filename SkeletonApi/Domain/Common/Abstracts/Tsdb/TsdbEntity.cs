using Microsoft.EntityFrameworkCore;
using SkeletonApi.Domain.Common.Interfaces.Tsdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Common.Abstracts.Tsdb
{
    [Keyless]
    public class TsdbEntity : ITsdbEntity
    {
    }
}