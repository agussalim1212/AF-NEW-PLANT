using Microsoft.EntityFrameworkCore;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Common.Interfaces;
using SkeletonApi.Domain.Entities;
using System.Reflection;

namespace SkeletonApi.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
          IDomainEventDispatcher dispatcher = null)
            : base(options)
        {
            _dispatcher = dispatcher;
        }

     
        public DbSet<Account> Accounts => Set<Account>();
       
        public DbSet<Machine> Machines => Set<Machine>();

        public DbSet<Subject> Subject => Set<Subject>();
        public DbSet<CategoryMachines> CategoryMachines => Set<CategoryMachines>(); 
        public DbSet<CategoryMachineHasMachine> CategoryMachineHasMachines => Set<CategoryMachineHasMachine>();
        public DbSet<SubjectHasMachine> subjectHasMachines => Set<SubjectHasMachine>();
        public DbSet<MaintenacePreventive> maintenacePreventives => Set<MaintenacePreventive>();
        public DbSet<MaintCorrective> maintenanceCorrectives => Set<MaintCorrective>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryMachineHasMachine>(m =>
            {
                m.HasKey(t => new { t.MachineId, t.CategoryMachineId });
                m.ToTable("CategoryMachineHasMachine");
            });

            modelBuilder.Entity<CategoryMachineHasMachine>(m =>
            {
                m.HasOne(t => t.Machine)
                .WithMany(t => t.CategoryMachineHasMachines)
                .HasForeignKey(t => t.MachineId);
                m.ToTable("CategoryMachineHasMachine");
            });

            modelBuilder.Entity<CategoryMachineHasMachine>(m =>
            {
                m.HasOne(t => t.CategoryMachine)
                .WithMany(t => t.CategoryMachineHasMachines)
                .HasForeignKey(t => t.CategoryMachineId);
                m.ToTable("CategoryMachineHasMachine");
            });

            modelBuilder.Entity<SubjectHasMachine>(m =>
            {
                m.HasKey(t => new { t.MachineId, t.SubjectId });
                m.ToTable("SubjectHasMachine");
            });

            modelBuilder.Entity<SubjectHasMachine>(m =>
            {
                m.HasOne(t => t.Machine)
                .WithMany(t => t.SubjectHasMachines)
                .HasForeignKey(t => t.MachineId);
                m.ToTable("SubjectHasMachine");
            });

            modelBuilder.Entity<SubjectHasMachine>(m =>
            {
                m.HasOne(t => t.Subject)
                .WithMany(t => t.SubjectHasMachines)
                .HasForeignKey(t => t.SubjectId);
                m.ToTable("SubjectHasMachine");
            });

            #region "Maintenance Preventive"
            modelBuilder.Entity<MaintenacePreventive>(m =>
            {
                m.HasKey(t => new { t.Id });
                m.ToTable("maintenacePreventives");
            });

            modelBuilder.Entity<MaintenacePreventive>(m =>
            {
                m.HasOne(t => t.machine)
                .WithMany(t => t.maintenacePreventives)
                .HasForeignKey(t => t.MachineId);
                m.ToTable("maintenacePreventives");
            });
            #endregion
            #region "Maintenance Corrective"
            modelBuilder.Entity<MaintCorrective>(m =>
            {
                m.HasKey(t => new { t.Id });
                m.ToTable("maintenanceCorrectives");
            });

            modelBuilder.Entity<MaintCorrective>(m =>
            {
                m.HasOne(t => t.machine)
                .WithMany(t => t.maintCorrectives)
                .HasForeignKey(t => t.MachineId);
                m.ToTable("maintenanceCorrectives");
            });
            #endregion

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // ignore events if no dispatcher provided
            if (_dispatcher == null) return result;

            // dispatch events only if save was successful
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

            return result;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}