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
        public DbSet<Dummy> Dummy => Set<Dummy>();
        public DbSet<CategoryMachines> CategoryMachines => Set<CategoryMachines>(); 
        public DbSet<CategoryMachineHasMachine> CategoryMachineHasMachines => Set<CategoryMachineHasMachine>();
        public DbSet<SubjectHasMachine> subjectHasMachines => Set<SubjectHasMachine>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<FrameNumberHasSubject> FrameNumberHasSubjects => Set<FrameNumberHasSubject>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FrameNumberHasSubject>(m =>
            {
                m.HasKey(t => new { t.SubjectId, t.FrameNumberId });
                m.ToTable("FrameNumberHasSubject");
            });

            modelBuilder.Entity<FrameNumberHasSubject>(m =>
            {
                m.HasOne(t => t.Subject)
                .WithMany(t => t.FrameNumberHasSubjects)
                .HasForeignKey(t => t.SubjectId);
                m.ToTable("FrameNumberHasSubject");
            });

            modelBuilder.Entity<FrameNumberHasSubject>(m =>
            {
                m.HasOne(t => t.FrameNumber)
                .WithMany(t => t.FrameNumberHasSubjects)
                .HasForeignKey(t => t.FrameNumberId);
                m.ToTable("FrameNumberHasSubject");
            });

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