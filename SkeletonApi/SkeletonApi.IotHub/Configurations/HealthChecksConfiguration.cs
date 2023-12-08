using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace SkeletonApi.IotHub.Configurations
{

    public static class HealthChecksConfiguration
    {
        public static void AddConfiguredHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                    .AddNpgSql(configuration.GetConnectionString("sqlConnection"))
                    .AddTcpHealthCheck(opts =>
                    {
                        opts.AddHost("backend.muhammadfahri.com", 1883);
                    }, timeout: TimeSpan.FromSeconds(5), name: "health-mqtt-broker");
            services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Queue Processing Service", "https://localhost:7145/health");
            }).AddSqliteStorage("Data Source = healthchecks.db");
        }
        public static void UseConfiguredHealthChecks(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecksUI();

            });
        }
    }
}
