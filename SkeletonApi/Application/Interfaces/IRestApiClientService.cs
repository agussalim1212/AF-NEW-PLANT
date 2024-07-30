using SkeletonApi.Application.DTOs.RestApiData;

namespace SkeletonApi.Application.Interfaces
{
    public interface IRestApiClientService
    {
        Task SendAsync(RestDataTraceability request);
    }
}