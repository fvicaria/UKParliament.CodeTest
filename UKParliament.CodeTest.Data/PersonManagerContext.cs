using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Serilog;

using UKParliament.CodeTest.Common.Entities;

namespace UKParliament.CodeTest.Data
{
    public class PersonManagerContext : DbContext
    {
        public DbSet<Person> People { get; protected set; }
        public DbSet<Department> Departments { get; protected set; }

        public PersonManagerContext(DbContextOptions<PersonManagerContext> options)
            : base(options)
        {
            // Constructor remains empty - configuration is done via DI
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Log.Information("Configuring model for PersonManagerContext...");  // Logging instead of Console.WriteLine

            // Apply entity configurations (using Fluent API in separate classes)
            modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentEntityTypeConfiguration());
        }
    }

    // EntityTypeConfiguration class for Person entity
    internal class PersonEntityTypeConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // Key and property constraints
            builder.HasKey(p => p.Id);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.DateOfBirth).IsRequired();
            builder.Property(p => p.Email).IsRequired().HasMaxLength(100);

            // Indexes for performance
            builder.HasIndex(p => p.Email).IsUnique();        // Unique index on Email
            builder.HasIndex(p => p.LastName);                // Index on LastName for quick lookup
            builder.HasIndex(p => p.DepartmentId);            // Index on foreign key for efficient joins/filters

            // Relationships (optimize foreign key relationship)
            builder.HasOne(p => p.Department)
                   .WithMany(d => d.Persons)                  // assuming Department.People navigation property exists
                   .HasForeignKey(p => p.DepartmentId)
                   .OnDelete(DeleteBehavior.Cascade);         // cascade delete for referential integrity
        }
    }

    // EntityTypeConfiguration class for Department entity
    internal class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);

            // Index on Name for faster lookups, and enforce unique department names
            builder.HasIndex(d => d.Name).IsUnique();
        }
    }

    // Dedicated seeding class to initialize data without duplication
    public static class PersonManagerContextSeeder
    {
        public static async Task SeedAsync(PersonManagerContext context)
        {
            try
            {
                bool added = false;

                if (!await context.Departments.AnyAsync())
                {
                    await context.Departments.AddRangeAsync(
                        new Department { Id = 1, Name = "Sales" },
                        new Department { Id = 2, Name = "Marketing" },
                        new Department { Id = 3, Name = "Finance" },
                        new Department { Id = 4, Name = "HR" }
                    );
                    added = true;
                }

                if (!await context.People.AnyAsync())
                {
                    await context.People.AddRangeAsync(
                        new Person
                        {
                            Id = 1,
                            FirstName = "Alice",
                            LastName = "Anderson",
                            DateOfBirth = new DateTime(1990, 1, 1),
                            Email = "alice@example.com",
                            DepartmentId = 1
                        },
                        new Person
                        {
                            Id = 2,
                            FirstName = "Bob",
                            LastName = "Baker",
                            DateOfBirth = new DateTime(1985, 12, 15),
                            Email = "bob@example.com",
                            DepartmentId = 2
                        }
                    );
                    added = true;
                }

                if (added)
                {
                    await context.SaveChangesAsync(); // Async
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database seeding: {ex.Message}");
                throw;
            }
        }
    }
}
