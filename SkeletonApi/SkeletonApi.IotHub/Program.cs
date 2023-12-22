using Microsoft.Extensions.Configuration;
using SkeletonApi.IotHub.Configurations;
using SkeletonApi.Persistence.IServiceCollectionExtensions;
using SkeletonApi.IotHub.Services;
using SkeletonApi.IotHub.Model;
using SkeletonApi.IotHub.Services.Handler;
using System.Reflection;
using SkeletonApi.IotHub.Services.Store;
using Microsoft.AspNetCore.Mvc;
using SkeletonApi.IotHub.Hubs;
using Microsoft.AspNetCore.SignalR;
using SkeletonApi.WebAPI.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Persistence.Repositories.Configuration;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHostedMqttClient(builder.Configuration);
builder.Services.AddConfiuredCors(builder.Configuration);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.AddScoped<IDapperReadDbConnection, DapperReadDbConnection>();
builder.Services.AddScoped<IDapperWriteDbConnection, DapperWriteDbConnection>();
//builder.Services.AddSignalR();
//builder.Services.AddObservablePipelines();
//builder.Services.AddSingleton<IUserRepository>();
builder.Services.AddSingleton<StatusMachineStore>();
//builder.Services.AddSingleton<Hub<IMachineHealthHub>, MachineHealthHub>();
builder.Services.AddHttpClient<IRestApiClientService, RestApiClientService>();

builder.Services.AddSingleton<IIoTHubEventHandler<MqttRawDataEncapsulation>,IotHubMqttEventHandler>();
builder.Services.AddSingleton<IotHubMachineHealthEventHandler, IotHubMachineHealthEventHandler>();

//builder.Services.AddSingleton<PipeServices>();
builder.Services.AddHostedService<PersistedConsumer>();
//builder.Services.AddConfiguredHealthChecks(builder.Configuration);
//builder.Services.AddJwtAuthentication(configuration);
builder.Services.AddSignalR(opt =>
{
    opt.EnableDetailedErrors = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

//var pipes = app.Services.GetRequiredService<PipeServices>();
//await pipes.Start();
app.UseCors("CorsPolicy");
//app.UseConfiguredSignalR()
app.UseRouting();
app.UseWebSockets();

//app.UseAuthentication();
//app.UseConfiguredCors();
app.UseAuthorization();
app.UseConfiguredSignalR();
//app.UseConfiguredHealthChecks();
//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateTime.Now.AddDays(index),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<BrokerHub>("/iothub");
    endpoints.MapHub<MachineHealthHub>("/machine-health-hub");
});
//app.MapGet("api/traceability-result-subject", (SubjectStore subjectStore) =>
//{
//    var traceabilityResultStore = subjectStore.GetTraceabilityResultStore();
//    var data = traceabilityResultStore.Subjects.Select(x=> new
//    {
//        process_part = x.Vid.Split("_").Last()
//    }).ToArray();
//    return data;

//});


app.MapGet("api/machine-health-subject", (StatusMachineStore machineStore) =>
{
    var data = machineStore.GetAllMachine();
    return data;

});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}