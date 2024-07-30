using SkeletonApi.IotHub.Settings;

namespace SkeletonApi.IotHub.Configurations
{
    public static class CorsConfiguration
    {
        public static void AddConfiuredCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSetting = new CorsSettings();
            configuration.GetSection(nameof(CorsSettings)).Bind(corsSetting);
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials() // allow credentials
                    .WithOrigins(corsSetting.AllowedHosts)
                    .WithExposedHeaders("x-download")
                    .WithExposedHeaders("x-pagination")
                    .SetIsOriginAllowed((hosts) => true));
            });
        }

        public static void UseConfiguredCors(this IApplicationBuilder app)
        {
            app.UseCors();
        }
    }
}