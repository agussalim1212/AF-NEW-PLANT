using Microsoft.AspNetCore.Builder;
using SkeletonApi.IotHub.Hubs;

namespace SkeletonApi.IotHub.Configurations
{
    public static class SignalRConfiguration
    {
        public static void UseConfiguredSignalR(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<BrokerHub>("/agenthub");
            });
        }
    }
}
