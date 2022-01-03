using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace DotNetCoreBoilerPlate.Models
{
    public partial class NetCoreBoilerPlateContext : DbContext
    {
        public string ConnectionString { get; set; }
        public NetCoreBoilerPlateContext()
        {
        }

        public NetCoreBoilerPlateContext(DbContextOptions<NetCoreBoilerPlateContext> options)
            : base(options)
        {
        }

        public NetCoreBoilerPlateContext(string connectionString) : base(GetOptions(connectionString)) { }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(ConnectionString)) optionsBuilder.UseSqlServer(ConnectionString);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var AddedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();
            if (AddedEntities.Count > 0)
            {
                AddedEntities.ForEach(E =>
                {
                    E.Property("CreatedAt").CurrentValue = DateTime.Now;
                    E.Property("UpdatedAt").IsModified = false;
                    E.Property("UpdatedBy").IsModified = false;
                });
            }

            var EditedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();
            if (EditedEntities.Count > 0)
            {
                EditedEntities.ForEach(E =>
                {
                    E.Property("CreatedAt").IsModified = false;
                    E.Property("CreatedBy").IsModified = false;
                    E.Property("UpdatedAt").CurrentValue = DateTime.Now;
                });
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual DbSet<BoilerTable> BoilerTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BoilerTable>(entity =>
            {
                entity.ToTable("BoilerTable");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.createdAt).HasColumnType("datetime");

                entity.Property(e => e.deletedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.updateBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.updatedAt).HasColumnType("datetime");
            });

            // For managing IsDeleted column to filter dataset

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    var isDeletedProperty = entityType.FindProperty("IsDeleted");
            //    if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
            //    {
            //        var parameter = Expression.Parameter(entityType.ClrType, "p");
            //        var filter = Expression.Lambda(Expression.Equal(Expression.Property(parameter, isDeletedProperty.PropertyInfo), Expression.Constant(false, typeof(bool))), parameter);
            //        entityType.SetQueryFilter(filter);
            //    }
            //}

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
