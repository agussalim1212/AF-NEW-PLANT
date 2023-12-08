using SkeletonApi.Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SkeletonApi.Infrastructure.Hubs
{
    public interface IDataHub
    {
        public ChannelReader<IEnumerable<MachineDto>> RealtimeMachine();
    }
}
