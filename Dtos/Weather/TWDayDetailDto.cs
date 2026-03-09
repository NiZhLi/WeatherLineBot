namespace WeatherBot.Dtos.Weather
{
    public class TWDayDetailDto
    {
        public string success { get; set; }
        public Records records { get; set; }
    }

    public class Records
    {
        public TWlocation[] Locations { get; set; }
    }

    public class TWlocation
    {
        public string DatasetDescription { get; set; }
        public string LocationsName { get; set; }
        public string Dataid { get; set; }
        public Location1[] Location { get; set; }
    }

    public class Location1
    {
        public string LocationName { get; set; }
        public string Geocode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Weatherelement[] WeatherElement { get; set; }
    }

    public class Weatherelement
    {
        public string ElementName { get; set; }
        public Time[] Time { get; set; }
    }

    public class Time
    {
        public DateTime DataTime { get; set; }
        public Elementvalue[] ElementValue { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class Elementvalue
    {
        public string Temperature { get; set; }
        public string RelativeHumidity { get; set; }
        public string ApparentTemperature { get; set; }
        public string WindSpeed { get; set; }
        public string BeaufortScale { get; set; }
        public string ProbabilityOfPrecipitation { get; set; }
        public string Weather { get; set; }
        public string WeatherCode { get; set; }
    }

}
