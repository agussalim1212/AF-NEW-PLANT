using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Application.Interfaces.Repositories.Dapper;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;
using SkeletonApi.Persistence.Repositories;
using SkeletonApi.Persistence.Repositories.Configuration;
using SkeletonApi.Persistence.Repositories.Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.IServiceCollectionExtensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddMappings();
            services.AddDbContext(configuration);
            services.AddRepositories();
           
        }

        //private static void AddMappings(this IServiceCollection services)
        //{
        //    services.AddAutoMapper(Assembly.GetExecutingAssembly());
        //}

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("sqlConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(connectionString,
                   builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services
                .AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork))
                .AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddTransient(typeof(IGenRepository<>), typeof(GenRepository<>))
                .AddTransient(typeof(IDataRepository<>), typeof(DataRepository<>))
                .AddScoped<DapperUnitOfWorkContext>()
                .AddTransient<ISubjectRepository, SubjectRepository>()
                .AddTransient<IMachinesRepository, MachinesRepository>()
                .AddTransient<ICategoryMachineRepository, CategoryMachinesRepository>()
                .AddTransient<IRoleRepository, RoleRepository>()
                .AddTransient<IAuthenticationUserRepository, AuthenticationRepository>()
                .AddTransient<IAccountRepository, AccountRepository>()
                .AddScoped<IEnginePartRepository, EnginePartRepository>()
                .AddTransient<IStatusMachineRepository, StatusMachineRepository>();
                

        }
    }
}