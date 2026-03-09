using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherBot.Services
{
    public class DomainMessageService
    {
        // 判斷天氣資訊並組合成訊息
        public string GetWeatherAdviceMessage(List<string> weatherInfo)
        {
            if (weatherInfo == null || weatherInfo.Count < 6)
            {
                return "無法取得完整的天氣資訊。";
            }

            // 1. 提取資訊 (依據 DomainWeatherService.GetTomorrowDetailAsync 的回傳順序)
            // Index 0: 溫度, 1: 相對濕度, 2: 體感溫度, 3: 蒲風級, 4: 3小時降雨機率, 5: 天氣現象
            var humidityList = ExtractValues(weatherInfo[1], "相對濕度");
            var tempApparentList = ExtractValues(weatherInfo[2], "體感溫度");
            var beaufortList = ExtractValues(weatherInfo[3], "蒲風級");
            var pop = weatherInfo[4].Replace("3小時降雨機率: ", "");
            var weatherPhenomenon = weatherInfo[5].Replace("天氣現象: ", "");

            if (!tempApparentList.Any())
            {
                return "天氣數據解析失敗。";
            }

            double tMax = tempApparentList.Max();
            double tMin = tempApparentList.Min();
            double deltaT = tMax - tMin;
            double hAvg = humidityList.Any() ? humidityList.Average() : 0;
            double wMax = beaufortList.Any() ? beaufortList.Max() : 0;

            // 2. 穿衣邏輯判斷
            // 2.1 基礎層判斷 (以 Tmax 為基準)
            string baseLayer = "";
            if (tMax > 26) baseLayer = "建議「短袖」為基礎。";
            else if (tMax > 19) baseLayer = "建議「薄長袖」或「短袖 + 薄外套」為基礎。"; // 20 < Tmax <= 26
            else baseLayer = "建議「長袖」為基礎。"; // Tmax <= 19

            // 2.2 溫差修正 (以 ΔT 為關鍵)
            string rangeAdvice = "";
            if (deltaT < 5) rangeAdvice = "建議「單一厚度穿法」。例如全天長袖。";
            else if (deltaT <= 10) rangeAdvice = "建議「內薄外厚」。提示：「早晚偏涼，中午可脫掉外套」。";
            else rangeAdvice = "建議「洋蔥式穿法」。提示：「內層短袖，外層需中度保暖（如輕羽絨或防風夾克）」。";

            // 3. 環境因素修正
            // 3.1 風速判斷 (Wmax >= 4 級)
            string windAdvice = "";
            if (wMax >= 4) windAdvice = "【防風建議】風力較強，外套請選擇防風材質。";

            // 3.2 濕度判斷 (Havg > 75%)
            string humidityAdvice = "";
            if (hAvg > 75)
            {
                if (tMax > 26) humidityAdvice = "【濕度提醒】環境潮濕，建議穿著吸濕排汗材質。";
                else humidityAdvice = "【濕度提醒】天氣濕冷，建議多穿一件輕量保暖層。";
            }

            // 組合訊息
            var message = $"【明日天氣詳細預報】\n" +
                          $"天氣現象：{weatherPhenomenon}\n" +
                          $"降雨機率：{pop}\n" +
                          $"體感溫度：{tMin}°C - {tMax}°C\n" +
                          $"最大風力：{wMax} 級\n\n" +
                          $"【穿衣建議】\n" +
                          $"{baseLayer}\n" +
                          $"{rangeAdvice}";

            if (!string.IsNullOrEmpty(windAdvice)) message += $"\n\n{windAdvice}";
            if (!string.IsNullOrEmpty(humidityAdvice)) message += $"\n\n{humidityAdvice}";

            return message;
        }

        private List<double> ExtractValues(string line, string prefix)
        {
            var dataPart = line.Replace($"{prefix}: ", "").Trim();
            if (string.IsNullOrEmpty(dataPart)) return new List<double>();

            return dataPart.Split(", ", StringSplitOptions.RemoveEmptyEntries)
                           .Select(v => double.TryParse(v, out var result) ? result : 0.0)
                           .ToList();
        }
    }
}
