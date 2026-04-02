using static WeatherBot.Dtos.Domain.WeatherTimePointDto;

namespace WeatherBot.Dtos.Domain;

// 1. 定義單一時間點的天氣細節
public record WeatherTimePointDto
{
    public record PointElement
    {
        public DateTime DataTime { get; init; }
        public string Temperature { get; init; }
        public string Humidity { get; init; }
        public string ApparentTemperature { get; init; }
        public string BeaufortScale { get; init; }
    }
    public record PeriodElement
    {
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public string PrecipitationProbability { get; init; }
        public string WeatherPhenomenon { get; init; }
    }
}

// 包含這些時間點的集合
public record WeatherDto
{
    // C# 12 集合運算式 []
    public IReadOnlyList<PointElement> TimePoints { get; init; } = [];
    public IReadOnlyList<PeriodElement> TimePeriod { get; init; } = [];
}