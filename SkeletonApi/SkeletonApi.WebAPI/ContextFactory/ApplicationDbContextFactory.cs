using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SkeletonApi.Persistence.Contexts;

namespace SkeletonApi.WebAPI.ContextFactory
{
 
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(configuration.GetConnectionString("sqlConnection"),
            b => b.MigrationsAssembly("SkeletonApi.WebAPI"));
            return new ApplicationDbContext(builder.Options);
        }
    }
}