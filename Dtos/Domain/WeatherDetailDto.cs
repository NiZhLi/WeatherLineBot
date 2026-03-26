namespace WeatherBot.Dtos.Domain
{
    public class WeatherDetailDto
    {
        public List<double> Temperatures { get; set; } = new();
        public List<double> Humidities { get; set; } = new();
        public List<double> ApparentTemperatures { get; set; } = new();
        public List<double> BeaufortScales { get; set; } = new();
        public List<string> PrecipitationProbabilities { get; set; } = new();
        public List<string> WeatherPhenomena { get; set; } = new();
    }
}
