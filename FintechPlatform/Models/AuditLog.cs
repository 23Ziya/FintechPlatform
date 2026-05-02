using System;

public class AuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; } // "AI Analizi Oluşturuldu", "Firma Güncellendi" vb.
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Detail { get; set; } // Teknik detaylar
}
