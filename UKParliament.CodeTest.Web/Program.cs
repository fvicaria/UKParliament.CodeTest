
using Serilog;
using UKParliament.CodeTest.Web.Services;

namespace UKParliament.CodeTest.Web
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
                // Add services to the container
                builder.Services.AddControllersWithViews();

                builder.Services.AddHttpClient<PersonManagerApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7003/api/"); // REST API Base URL
                });

                var app = builder.Build();


                // Middleware pipeline
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                await app.RunAsync();
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
