using Microsoft.AspNetCore.Diagnostics;
using SkeletonApi.Application.Common.Exceptions;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Domain.ErrorModel;
using System.Net;
using System.Text.Json;
using ILogger = Serilog.ILogger;

namespace SkeletonApi.WebAPI.Extensions
{
    public static class ErrorHandlerExtensions
    {
        public static void UseErrorHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature == null) return;

                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.ContentType = "application/json";

                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        BadRequestException => (int)HttpStatusCode.BadRequest,
                        OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
                        NotFoundException => (int)HttpStatusCode.NotFound,
                        _ => (int)HttpStatusCode.InternalServerError
                    };
                    logger.Error($"Something went wrong: {contextFeature.Error}");

                    var errorResponse = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = contextFeature.Error.GetBaseException().Message
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                });
            });
        }
    }

    //public static class ExceptionMiddlewareExtensions
    //{
    //    public static void ConfigureExceptionHandler(this WebApplication app,
    //    ILoggerManager logger)
    //    {
    //        app.UseExceptionHandler(appError =>
    //        {
    //            appError.Run(async context =>
    //            {
    //                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //                context.Response.ContentType = "application/json";
    //                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
    //                if (contextFeature != null)
    //                {
    //                    logger.LogError($"Something went wrong: {contextFeature.Error}");
    //                    await context.Response.WriteAsync(new ErrorDetails()
    //                    {
    //                        StatusCode = context.Response.StatusCode,
    //                        Message = "Internal Server Error.",
    //                    }.ToString());
    //                }
    //            });
    //        });
    //    }
    //}
}
