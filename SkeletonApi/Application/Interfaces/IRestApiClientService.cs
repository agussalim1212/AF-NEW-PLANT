using SkeletonApi.Application.DTOs.Email;
using SkeletonApi.Application.DTOs.RestApiData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces
{
    public interface IRestApiClientService
    {
        Task SendAsync(RestDataTraceability request);

    }
}
