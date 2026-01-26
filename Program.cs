using Serilog;
using Serilog.Events;
using WeatherBot.Services;

namespace WeatherBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt",
                                rollingInterval: RollingInterval.Day,
                                retainedFileCountLimit: 720)
                .CreateLogger();

            try
            { 
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddSerilog();
                builder.Services.AddHttpClient();

                builder.Services.AddScoped<WeatherOpenDataService>();
                builder.Services.AddScoped<LineBotService>();
                builder.Services.AddScoped<DomainWeatherService>();

                var app = builder.Build();

                //var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
                //app.Urls.Add($"http://0.0.0.0:{port}");

                //// 顯示伺服器啟動訊息
                //Console.WriteLine($"Server running on port {port}");
                //Console.WriteLine(" Server listening on all available interfaces");

                // Railway Environment Variables Debugging
                Console.WriteLine(" Railway Environment Variables:");
                Console.WriteLine("RAILWAY_ENVIRONMENT: " + Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT"));
                Console.WriteLine("RAILWAY_SERVICE_NAME: " + Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME"));
                Console.WriteLine("RAILWAY_PUBLIC_DOMAIN: " + Environment.GetEnvironmentVariable("RAILWAY_PUBLIC_DOMAIN"));
                Console.WriteLine("RAILWAY_PRIVATE_DOMAIN: " + Environment.GetEnvironmentVariable("RAILWAY_PRIVATE_DOMAIN"));

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseSerilogRequestLogging();
                //app.UseHttpsRedirection(); // if deploy doesn't work annotate this line
                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception er)
            {
                Log.Fatal(er, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
