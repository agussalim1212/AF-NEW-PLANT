using Microsoft.OpenApi.Models;

namespace SkeletonApi.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        //public static void ConfigureCors(this IServiceCollection services) =>
        //    services.AddCors(opts =>
        //    {
        //        opts.AddPolicy("CorsPolicy", builder =>
        //            builder.AllowAnyOrigin()
        //            .AllowAnyMethod()
        //            .AllowAnyHeader());
        //    });

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options =>
            {
            });

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SkeletonAPI",
                    Version = "v1"
                });
                s.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "SkeletonAPI",
                    Version = "v2"
                });
            });
        }
    }
}
