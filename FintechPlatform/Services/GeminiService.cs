using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FintechPlatform.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        // Not: API Key ve URL'ne dokunulmadı.
        private const string ApiKey = "AIzaSyD5yvbGsPsDyjllQGOoMbJ_gzSNtXJSFlU";

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ProcessDocumentAsync(string base64Data, string contentType)
        {
            // Model ismini en güncel hal olan gemini-1.5-flash yapalım
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={ApiKey}";

            var payload = new
            {
                contents = new[] {
            new {
                parts = new object[] {
                    new { text = "Bu Ticaret Sicil Gazetesi dökümanını analiz et. 'sirketUnvani', 'ticaretSicilNo', 'kurulusTarihi', 'yetkiliKisi' (Yönetim Kurulu Başkanı veya Temsilci) ve 'yillikCiro' (Sermaye tutarı) alanlarını JSON olarak döndür. Sadece JSON döndür." },
                    new { inline_data = new { mime_type = contentType, data = base64Data } }
                }
            }
        },
                generationConfig = new { response_mime_type = "application/json" }
            };

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseString);
                    var rawText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                    if (!string.IsNullOrEmpty(rawText) && rawText.Contains("```"))
                        rawText = rawText.Replace("```json", "").Replace("```", "").Trim();

                    return rawText ?? "{}";
                }
                else
                {
                    // Hata durumunda nedenini dön ki UI'da görebilelim
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    return "{\"error\": \"API Hatası: " + response.StatusCode + "\", \"detail\": " + JsonSerializer.Serialize(errorDetail) + "}";
                }
            }
            catch (Exception ex)
            {
                return "{\"error\": \"Kod Hatası\", \"detail\": \"" + ex.Message + "\"}";
            }
        }
    }
}