using Serilog;
using Serilog.Events;
using WeatherBot.Services;
using WeatherBot.Services.LineMessaging;
using WeatherBot.Services.LineMessaging.Handlers;
using WeatherBot.Services.LineMessaging.Strategies;

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
                builder.Services.AddScoped<DomainWeatherService>();
                builder.Services.AddSingleton<ITaiwanLocationResolver, TaiwanLocationResolver>();
                builder.Services.AddScoped<IWebhookEventHandler, MessageWebhookEventHandler>();
                builder.Services.AddScoped<IMessageStrategy, KeywordReplyMessageStrategy>();
                builder.Services.AddScoped<IMessageStrategy, LocationMessageStrategy>();
                builder.Services.AddScoped<IMessageStrategy, CityWeatherMessageStrategy>();
                builder.Services.AddScoped<LineBotService>();

                var app = builder.Build();

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
