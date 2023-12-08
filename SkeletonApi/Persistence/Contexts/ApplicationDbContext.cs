using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Domain.Common.Abstracts;
using SkeletonApi.Domain.Common.Interfaces;
using SkeletonApi.Domain.Entities;
using System.Data;


namespace SkeletonApi.Persistence.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>,
    UserRole, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , IDomainEventDispatcher dispatcher = null)
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
        public DbSet<FrameNumberHasSubjects> FrameNumberHasSubjects => Set<FrameNumberHasSubjects>();
        public DbSet<FrameNumber> FrameNumbers => Set<FrameNumber>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<ActivityUser> ActivityUsers => Set<ActivityUser>();
        public DbSet<MaintenacePreventive> MaintenacePreventives => Set<MaintenacePreventive>();
        public DbSet<MaintCorrective> MaintCorrectives => Set<MaintCorrective>();
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId);
            });


                modelBuilder.Entity<Role>()
               .HasMany(e => e.Permissions)
               .WithOne(e => e.Role)
               .HasForeignKey(e => e.RoleId)
               .IsRequired(false);

            modelBuilder.Entity<FrameNumberHasSubjects>(m =>
            {
                m.HasKey(t => new { t.SubjectId, t.FrameNumberId });
                m.ToTable("FrameNumberHasSubject");
            });

            modelBuilder.Entity<FrameNumberHasSubjects>(m =>
            {
                m.HasOne(t => t.Subject)
                .WithMany(t => t.FrameNumberHasSubjects)
                .HasForeignKey(t => t.SubjectId);
                m.ToTable("FrameNumberHasSubject");
            });

            modelBuilder.Entity<FrameNumberHasSubjects>(m =>
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

            modelBuilder.Entity<ActivityUser>(
            eb =>
            {
                eb.Property(b => b.Id).HasColumnName("id").HasColumnType("uuid");
                eb.Property(b => b.UserName).HasColumnName("username").HasColumnType("text");
                eb.Property(b => b.LogType).HasColumnName("logtype").HasColumnType("text");
                eb.Property(b => b.DateTime).HasColumnName("datetime").HasColumnType("timestamp");
            });

            modelBuilder.Entity<Account>(
            eb =>
            {
                eb.Property(b => b.Username).HasColumnName("username").HasColumnType("text");
                eb.Property(b => b.PhotoURL).HasColumnName("photo_url").HasColumnType("text");
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

        public IDbConnection Connection { get; }
    }
}