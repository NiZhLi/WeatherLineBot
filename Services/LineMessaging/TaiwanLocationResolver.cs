using System;
using System.Collections.Generic;

namespace WeatherBot.Services.LineMessaging
{
    public interface ITaiwanLocationResolver
    {
        string Resolve(string? messageText);
    }

    public class TaiwanLocationResolver : ITaiwanLocationResolver
    {
        private static readonly HashSet<string> ValidLocations = new()
        {
            "臺北", "新北", "桃園", "臺中", "臺南", "高雄",
            "基隆", "新竹", "嘉義", "苗栗", "彰化", "南投",
            "雲林", "屏東", "宜蘭", "花蓮", "臺東", "澎湖",
            "金門", "連江"
        };

        private static readonly HashSet<string> CityNames = new()
        {
            "臺北", "新北", "桃園", "臺中", "臺南", "高雄", "基隆"
        };

        private static readonly HashSet<string> CountyNames = new()
        {
            "苗栗", "彰化", "南投", "雲林", "屏東", "宜蘭", "花蓮", "臺東", "澎湖", "金門", "連江"
        };

        private static readonly HashSet<string> CityAndCounty = new()
        {
            "新竹", "嘉義"
        };

        public string Resolve(string? messageText)
        {
            if (string.IsNullOrWhiteSpace(messageText) || messageText.Length > 1000)
            {
                return messageText ?? string.Empty;
            }

            var normalizedMessage = messageText.Replace("台", "臺");

            foreach (var location in ValidLocations)
            {
                if (!normalizedMessage.Contains(location, StringComparison.Ordinal))
                {
                    continue;
                }

                if (CityNames.Contains(location))
                {
                    return location + "市";
                }

                if (CountyNames.Contains(location))
                {
                    return location + "縣";
                }

                if (CityAndCounty.Contains(location))
                {
                    return location + "市";
                }
            }

            return messageText;
        }
    }
}
