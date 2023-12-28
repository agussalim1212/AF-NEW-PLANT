using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkeletonApi.Application.Interfaces;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Persistence.Contexts;
using SkeletonApi.Persistence.Repositories;


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
                .AddTransient<ISubjectRepository, SubjectRepository>()
                .AddTransient<IMachinesRepository, MachinesRepository>()
                .AddTransient<ICategoryMachineRepository, CategoryMachinesRepository>()
                .AddTransient<IRoleRepository, RoleRepository>()
                .AddTransient<IAuthenticationUserRepository, AuthenticationRepository>()
                .AddTransient<IAccountRepository, AccountRepository>()
                .AddTransient<IStatusMachineRepository, StatusMachineRepository>()
                .AddTransient<IDetailGensubRespository, DetailGensubRepository>()
                .AddTransient<IDetailAssyUnitRepository, DetailAssyUnitRepository>()
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<ISettingRepository, SettingRepository>()
                .AddTransient<IDetailAssyWheelLineRepository, DetailAssyWheelLineRepository>();
                

        }
    }
}