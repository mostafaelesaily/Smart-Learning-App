using System.Text;
using System.Text.Json;

namespace Smart_Learning_App.Integrations.Ai.Implementations
{
    public class GeminiIntegration : IAiService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly ILogger<GeminiIntegration> logger;

        public GeminiIntegration(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<GeminiIntegration> logger)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            var apiKey = configuration["Integrations:Gemini:ApiKey"];
            var model = configuration["Integrations:Gemini:Model"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                logger.LogCritical("Gemini API key is not configured.");
                throw new InvalidOperationException("Gemini ApiKey is missing.");
            }
               


            if (string.IsNullOrWhiteSpace(model))
            {
                logger.LogCritical("Gemini model is not configured.");
                throw new InvalidOperationException("Gemini Model is missing.");
            }

            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            try
            {

                logger.LogInformation("Sending request to Gemini API.");

                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(responseBody);

                return document
                    .RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString()!;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while sending request to Gemini API.");
                throw;
            }

           
        }
    }
}