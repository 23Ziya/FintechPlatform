using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FintechPlatform.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "AIzaSyDAasUy_1YMZXoRmDp0h-0UWAz8eDJ-APk"; // Google AI Studio'dan aldığın anahtar

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ProcessDocumentAsync(string base64Data, string contentType)
        {
            // Gemini 1.5 Flash modeli hem hızlı hem de döküman okumada çok başarılı
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={ApiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = @"Bu belgeyi analiz et ve verileri şu JSON formatında döndür. 
                                          Sadece JSON döndür, başka açıklama yapma.... kurulusTarihi alanına dökümanda geçen 'Yıl' veya 'Onay Zamanı'ndaki yılı yaz ...
                                          Format: 
                                          {
                                            ""sirketUnvani"": """", 
                                            ""vergiNumarasi"": """", 
                                            ""ticaretSicilNo"": """", 
                                            ""kurulusTarihi"": """", 
                                            ""faaliyetAlani"": """", 
                                            ""yetkiliKisi"": """", 
                                            ""yillikCiro"": """", 
                                            ""vergiMatrahi"": """", 
                                            ""ticariKar"": """", 
                                            ""kkeg"": """"
                                          }" },
                            new { inline_data = new { mime_type = contentType, data = base64Data } }
                        }
                    }
                },
                generationConfig = new { response_mime_type = "application/json" } // JSON çıktısı zorunlu
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);
                // Gemini'ın yanıt yapısı içinden metni çekiyoruz
                return doc.RootElement.GetProperty("candidates")[0]
                                      .GetProperty("content")
                                      .GetProperty("parts")[0]
                                      .GetProperty("text").GetString();
            }

            return "Hata: " + response.StatusCode;
        }
    }
}