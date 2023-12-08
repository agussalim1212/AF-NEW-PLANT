using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SkeletonApi.Application.Extensions;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Infrastructure.Extensions;
using SkeletonApi.Persistence.Contexts;
using SkeletonApi.Persistence.IServiceCollectionExtensions;
using SkeletonApi.Persistence.Repositories;
using SkeletonApi.Persistence.Repositories.Configuration;
using SkeletonApi.Presentation.ActionFilter;
using SkeletonApi.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureIdentity();
builder.Services.AddAuthentication();
//builder.Services.ConfigurePermissionService();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer();
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.ConfigureApiBehavior();
builder.Services.ConfigureCorsPolicy(builder.Configuration);
builder.Services.AddScoped<IDapperReadDbConnection, DapperReadDbConnection>();
builder.Services.AddScoped<IDapperWriteDbConnection, DapperWriteDbConnection>();
builder.Services.AddScoped<AuditLoggingFilter>();
builder.Services.AddScoped<AuditRepository>();
builder.Services.ConfigureIISIntegration();
builder.Services.AddHostedMqttClient(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddControllers(
    config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
        //config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
        config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
    }).AddXmlDataContractSerializerFormatters()
    //.AddCustomCSVFormatter()
    .AddApplicationPart(typeof(SkeletonApi.Presentation.AssemblyReference).Assembly);

var app = builder.Build();
//var logger = Log.Logger;
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();
// Configure the HTTP request pipeline.
var serviceScope = app.Services.CreateScope();
var dataContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
//dataContext?.Database.EnsureCreated();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "SkeletonAPI v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "SkeletonAPI v2");
});

app.UseErrorHandler(Log.Logger);
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");
//app.UseResponseCaching();
//app.UseHttpCacheHeaders();

app.UseRouting();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
