using System;
using System.Collections.Generic;
using System.Linq;
using WeatherBot.Dtos.Domain;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services
{
    public class DomainMessageService
    {
        public string GetWeatherMessage(WeatherDetailDto weatherInfo)
        {
            var humidityList = weatherInfo.Humidities;
            var tempApparentList = weatherInfo.ApparentTemperatures;
            var beaufortList = weatherInfo.BeaufortScales;
            var pop = weatherInfo.PrecipitationProbabilities;
            var weatherPhenomenon = weatherInfo.WeatherPhenomena;

            double tMin = weatherInfo.Temperatures.Min();
            double tMax = weatherInfo.Temperatures.Max();
            double deltaT = tMax - tMin;


            string pheAround = GetAroundPhe(weatherPhenomenon);

            var message = $"【天氣預報】\n" +
                          $"最高溫：{tMax}\n" +
                          $"最低溫：{tMin}\n" +
                          $"天氣現象：{pheAround}\n"
                          //$"平均降雨機率：{GetAroundPop(pop)}%"
                          ;

            return message;
        }

        // 判斷天氣資訊並組合成訊息
        public string GetDressAdviceMessage(WeatherDetailDto weatherInfo)
        {
            if (weatherInfo == null)
            {
                return "無法取得完整的天氣資訊。";
            }

            // 1. 提取資訊
            var humidityList = weatherInfo.Humidities;
            var tempApparentList = weatherInfo.ApparentTemperatures;
            var beaufortList = weatherInfo.BeaufortScales;
            var pop = weatherInfo.PrecipitationProbabilities;
            var weatherPhenomenon = weatherInfo.WeatherPhenomena;

            double taMax = tempApparentList.Max();
            double taMin = tempApparentList.Min();
            double deltaTa = taMax - taMin;

            double tMin = weatherInfo.Temperatures.Min();
            double tMax = weatherInfo.Temperatures.Max();
            double deltaT = tMax - tMin;

            double hAvg = humidityList.Average();
            double wMax = beaufortList.Max();

            // 2. 穿衣邏輯判斷
            // 2.1 基礎層判斷 (以 taMax 為基準)
            //string baseLayer = "";
            //if (taMax < 22) baseLayer = "建議「短袖」為基礎。";
            //else baseLayer = "建議「長袖」為基礎。";

            // 2.2 溫差修正 (以 ΔT 為關鍵)
            string rangeAdvice = "";
            if (tMax < 30 && tMin>=20)
            {
                rangeAdvice = "「內薄外厚」，早晚偏涼，中午可脫掉外套」。";
                
            }else if(tMax < 30 && tMin < 20)
            {
                if (deltaT >= 10) rangeAdvice = "「溫差大，外層需中度保暖（如防風夾克）」。";
                else rangeAdvice = "「建議選擇輕薄透氣衣料（如薄外套或罩衫）」。";
            }
            else if (tMax > 30)
            {
                if (deltaT >= 8) rangeAdvice = "「需著重防曬，並透氣穿著，注意補水」。";
                else rangeAdvice = "「建議選擇輕薄透氣衣料（如薄外套或罩衫）」。";
            }

            // 3. 環境因素修正
            // 3.1 風速判斷 (Wmax >= 4 級)
            string windAdvice = "";
            if (wMax >= 5) windAdvice = "【防風建議】風力較強，外套請選擇防風材質。";

            // 3.2 濕度判斷 (Havg > 75%)
            string humidityAdvice = "";
            if (hAvg > 75)
            {
                if (tMin >= 26) humidityAdvice = "【濕度提醒】環境潮濕，建議穿著吸濕排汗材質。";
                else humidityAdvice = "【濕度提醒】天氣濕冷，建議多穿一件輕量保暖層。";
            }

            //// 4. 天氣現象與降雨機率
            //foreach (var phe in weatherPhenomenon)
            //{

            //}

            // 組合訊息
            var message = $"【穿衣建議】\n" +
                          //$"{baseLayer}\n" +
                          $"{rangeAdvice}\n" +
                          $"{humidityAdvice}";

            return message;
        }

        string GetAroundPhe(List<string> weatherPhenomenon)
        {
            var message = "";
            var sunCounter = 0;
            foreach (var phe in weatherPhenomenon)
            {
                if (phe.Contains("雨"))
                {
                    message += "【降雨提醒】有降雨現象，建議攜帶雨具。";
                }
                if (phe.Contains("晴"))
                {
                    sunCounter++;
                    if (sunCounter >= (weatherPhenomenon.Count)/2)
                    {
                        message += "大致晴朗。";
                        break;
                    }
                }
            }

            return message;
        }
        //string GetAroundPop(List<string> weatherPop)
        //{
        //    var totoalPop = 0;
        //    foreach (var pop in weatherPop)
        //    {
        //        if(tot);
        //    }
            

        //    return $"{totoalPop/ weatherPop.Count}";
        //}
    }
}
