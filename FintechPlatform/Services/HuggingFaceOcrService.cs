using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FintechPlatform.Services
{
    public class HuggingFaceOcrService
    {
        private readonly HttpClient _httpClient;

        public HuggingFaceOcrService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // HuggingFaceOcrService.cs içindeki temel mantık
        public async Task<string> ProcessFileAsync(string base64Data, string contentType)
        {
            string baseUrl = "https://merterbak-deepseek-ocr-demo.hf.space/gradio_api/call/run";

            // [0] ve [1] indisli parametrelerin her ikisine de dosyayı gönderiyoruz
            var fileData = new { path = $"data:{contentType};base64,{base64Data}", meta = new { _type = "gradio.FileData" } };
            var payload = new { data = new object[] { fileData, fileData, "📋 Markdown", "Analyze this tax document", 1 } };

            var postResponse = await _httpClient.PostAsync(baseUrl, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            var eventData = await postResponse.Content.ReadAsStringAsync();
            string eventId = JsonDocument.Parse(eventData).RootElement.GetProperty("event_id").GetString();

            // AI'nın dökümanı işlemesi için bir süre bekle ve sonucu çek
            await Task.Delay(5000);
            var getResponse = await _httpClient.GetAsync($"{baseUrl}/{eventId}");
            var rawContent = await getResponse.Content.ReadAsStringAsync();

            // SSE (Server-Sent Events) formatını temizleme
            if (rawContent.Contains("data:"))
            {
                var parts = rawContent.Split("data: ");
                return parts.Last().Split("\n")[0]; // En güncel ve temiz veri dizisini alır
            }
            return "Analiz başarısız.";
        }
    }
}