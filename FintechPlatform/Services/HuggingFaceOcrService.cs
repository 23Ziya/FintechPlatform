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

        public async Task<string> ProcessFileAsync(string base64Data, string contentType)
        {
            // 1. ADIM: İŞLEMİ BAŞLAT (POST)
            // Not: Adresi dökümandaki gibi 'gradio_api/call/run' olarak güncelledik
            string postUrl = "https://merterbak-deepseek-ocr-demo.hf.space/gradio_api/call/run";

            var payload = new
            {
                data = new object[] {
                    null, // [0] Input Image
                    new {
                        path = $"data:{contentType};base64,{base64Data}", // [1] Dosya verisi
                        meta = new { _type = "gradio.FileData" }
                    },
                    "📋 Markdown", // [2] Task
                    "Analiz et ve verileri çıkar", // [3] Prompt
                    1 // [4] Page Number
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var postResponse = await _httpClient.PostAsync(postUrl, content);

            if (!postResponse.IsSuccessStatusCode)
                return $"POST Hatası: {postResponse.StatusCode}";

            // Dönen JSON'dan event_id'yi alıyoruz
            var postResult = await postResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(postResult);
            string eventId = doc.RootElement.GetProperty("event_id").GetString();

            // 2. ADIM: SONUCU AL (GET)
            // Aldığımız eventId ile sonuca gidiyoruz
            string getUrl = $"https://merterbak-deepseek-ocr-demo.hf.space/gradio_api/call/run/{eventId}";

            // AI'nın dökümanı işlemesi için kısa bir bekleme (opsiyonel ama sağlıklı olur)
            await Task.Delay(2000);

            var getResponse = await _httpClient.GetAsync(getUrl);
            if (getResponse.IsSuccessStatusCode)
            {
                var finalResult = await getResponse.Content.ReadAsStringAsync();
                // Not: Gradio bazen "data: ..." şeklinde Server-Sent Events döner. 
                // Hackathon MVP'si için gelen ham metni doğrudan döndürebilirsin.
                return finalResult;
            }

            return "Sonuç alma hatası: " + getResponse.StatusCode;
        }
    }
}