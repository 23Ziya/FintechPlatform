using Microsoft.AspNetCore.Http;

namespace FintechPlatform.Models
{
    public class OcrViewModel
    {
        public IFormFile? UploadedFile { get; set; }
        public string? OcrResult { get; set; }

        // --- Şirket Temel Bilgileri (İsterlere Göre) ---
        public string? SirketUnvani { get; set; }
        public string? VergiNumarasi { get; set; }
        public string? TicaretSicilNo { get; set; }
        public string? KurulusTarihi { get; set; } // Beyannamedeki "Yili" alanı[cite: 2, 3]
        public string? FaaliyetAlani { get; set; }
        public string? YetkiliKisi { get; set; }
        public string? IletisimBilgileri { get; set; }
        public string? KayitliAdres { get; set; }
        public string? YillikCiro { get; set; } // "Net Satışlar" üzerinden

        // --- Beyanname Rakamları (AI Analizi İçin) ---
        public string? VergiMatrahi { get; set; } // Beyannamedeki Safi Geçici Vergi Matrahı[cite: 2, 3]
        public string? TicariKar { get; set; }
        public string? KKEG { get; set; }

        // --- Sözleşme Bilgileri (Manuel Giriş) ---
        public string? SozlesmeBedeli { get; set; }
        public string? SozlesmeBaslangic { get; set; }
        public string? SozlesmeBitis { get; set; }
        public string? SozlesmeTuru { get; set; } // Rapor, Analiz, Sistem, Diger
    }
}