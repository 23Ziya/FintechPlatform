using Microsoft.AspNetCore.Http;

namespace FintechPlatform.Models
{
    public class OcrViewModel
    {
        // Kullanıcının yükleyeceği dosya
        public IFormFile? UploadedFile { get; set; }

        // AI'dan dönecek Markdown veya metin sonucu
        public string? OcrResult { get; set; }
    }
}