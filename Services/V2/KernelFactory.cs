using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace WeatherBot.Services.V2
{
    public class KernelFactory(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public Kernel Create()
        {
            var apiKey = _configuration["AI:Gemini:ApiKey"];
            var modelId = _configuration["AI:Gemini:ModelId"];

            var kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.AddGoogleAIGeminiChatCompletion(
                modelId: modelId,
                apiKey: apiKey);

            return kernelBuilder.Build();
        }
    }
}
