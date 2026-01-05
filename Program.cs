using WeatherBot.Services;

namespace WeatherBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();

            builder.Services.AddScoped<WeatherOpenDataService>();
            builder.Services.AddScoped<LineBotService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection(); // if deploy doesn't work annotate this line

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
