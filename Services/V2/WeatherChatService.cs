using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace WeatherBot.Services.V2
{
    public class WeatherChatService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<WeatherChatService> _logger;

        public WeatherChatService(Kernel kernel, WeatherPlugin weatherPlugin, ILogger<WeatherChatService> logger)
        {
            _kernel = kernel;
            _logger = logger;

            _kernel.Plugins.AddFromObject(weatherPlugin, "weather");
        }

        // prompt
        private const string SystemPrompt = @"
            根據氣象資訊提供使用者穿衣建議，並遵守以下規則：

            1. 若使用者提到台灣地名（如：台中、台北、台南、台東等），在傳遞給天氣工具時，請一律將「台」轉換為正體字「臺」。
            2. 若使用者只輸入地名縮寫（例如：臺中、臺北），請自動補上「市」或「縣」（例如：臺中市、臺北市、臺南市、臺東縣）。
            3. 如果使用者沒有指定日期，預設查詢今日12小時後的天氣。
            4. 根據天氣資訊提供穿衣建議，並說明理由，語氣貼心。
            5. 穿衣建議首要判斷該氣溫與時間段、天氣現象，再者是濕度、風速等其他氣象因素。
            6. 在訊息開頭簡短提及預報時間段。
            ";

        public async Task<string> ChatAsync(string userInput)
        {
            var executionSettings = new GeminiPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var promptTemplate = SystemPrompt + "\n\n使用者的問題：{{$userInput}}";

            var arguments = new KernelArguments(executionSettings)
            {
                { "userInput", userInput }
            };

            var result = await _kernel.InvokePromptAsync(promptTemplate, arguments);
            return result.ToString();
        }
    }
}
