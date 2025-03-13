using Microsoft.EntityFrameworkCore;

using Serilog;

using UKParliament.CodeTest.Data;
using UKParliament.CodeTest.Data.Repositories;
using UKParliament.CodeTest.Common.Interfaces;
using UKParliament.CodeTest.Services;

namespace UKParliament.CodeTest.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog for structured logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/app_log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            var logger = Log.ForContext<Program>();
            logger.Information("Starting application...");

            try
            {
                // Register DbContext with InMemory Database (or replace with SQL if needed)
                builder.Services.AddDbContext<PersonManagerContext>(options =>
                {
                    options.UseInMemoryDatabase("PersonManager"); // Replace with real DB if needed
                    options.LogTo(msg => logger.Information(msg), LogLevel.Information);
                });

                // Register Repositories
                builder.Services.AddScoped<IPersonRepository, EfPersonRepository>();

                // Register Services
                builder.Services.AddScoped<IPersonService, PersonService>();
                builder.Services.AddScoped<IDepartmentService, DepartmentService>();

                // Register Controllers
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                // Ensure Database is Created
                using (var scope = app.Services.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetRequiredService<PersonManagerContext>();
                    await ctx.Database.EnsureCreatedAsync();

                    // Seed Data
                    await PersonManagerContextSeeder.SeedAsync(ctx);

                    logger.Information("Database initialized.");
                }

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Application terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
