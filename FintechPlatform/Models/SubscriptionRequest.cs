namespace FintechPlatform.Models
{
    public class SubscriptionRequest
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string UserId { get; set; } // Talebi yapan kullanıcı
        public string PackageType { get; set; } // "AI Analiz", "Uzman Görüşü" veya "Premium Bundle"
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Beklemede"; // Beklemede, Onaylandı, Reddedildi
    }
}
