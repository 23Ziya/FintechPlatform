using Microsoft.AspNetCore.Http;

namespace FintechPlatform.Models
{
    public class OcrViewModel
    {
        // Kullanıcının yükleyeceği dosya
        public IFormFile? UploadedFile { get; set; }

        // AI'dan dönecek Markdown veya metin sonucu
        public string? OcrResult { get; set; }

        // Beyannameden çekilecek spesifik alanlar-yenileri eklenecek
        public string? TicariKar { get; set; }
        public string? KKEG { get; set; }
        public string? VergiMatrahi { get; set; }
    }
}