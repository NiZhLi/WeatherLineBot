namespace WeatherBot.Dtos
{
    public class WeatherResponseDto
    {
        public string success { get; set; }
        //public Result result { get; set; }
        public Records records { get; set; }
    }

    //public class Result
    //{
    //    public string resource_id { get; set; }
    //    public Field[] fields { get; set; }
    //}

    //public class Field
    //{
    //    public string id { get; set; }
    //    public string type { get; set; }
    //}

    public class Records
    {
        public string datasetDescription { get; set; }
        public Location[] location { get; set; }
    }

    public class Location
    {
        public string locationName { get; set; }
        public Weatherelement[] weatherElement { get; set; }
    }

    public class Weatherelement
    {
        public string elementName { get; set; }
        public Time[] time { get; set; }
    }

    public class Time
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public Parameter parameter { get; set; }
    }

    public class Parameter
    {
        public string parameterName { get; set; }
        public string parameterValue { get; set; }
        public string parameterUnit { get; set; }
    }
}
