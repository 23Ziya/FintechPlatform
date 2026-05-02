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

        // Resmi alıp API'ye gönderen metod
        public async Task<string> ProcessImageAsync(string base64Image)
        {
            // Hugging Face Space'in arkadaki gerçek API adresi (Genellikle cURL sekmesinde yazar)
            string apiUrl = "https://merterbak-deepseek-ocr-demo.hf.space/api/predict";

            // Gradio API'leri genelde "data" dizisi içinde veriyi bekler.
            // Data formatı: "data:image/jpeg;base64,YOUR_BASE64_STRING"
            var payload = new
            {
                data = new[] { $"data:image/jpeg;base64,{base64Image}" }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                // Gelen JSON'u parse edip içindeki Markdown metnini döneceğiz (Bunu API'nin dönüşüne göre ayarlayacağız)
                return responseString;
            }

            return "API Hatası: " + response.StatusCode;
        }
    }
}