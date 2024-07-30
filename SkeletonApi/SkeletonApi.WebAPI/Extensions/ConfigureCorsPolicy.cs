using SkeletonApi.WebAPI.Setting;

namespace SkeletonApi.WebAPI.Extensions
{
    public static class CorsPolicyExtensions
    {
        public static void ConfigureCorsPolicy(this IServiceCollection services, IConfiguration configuration)
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
    }
}