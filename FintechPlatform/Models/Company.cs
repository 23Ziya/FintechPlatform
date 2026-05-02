using System;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } // Şirket Ünvanı
    public string TaxNumber { get; set; } // Vergi Numarası
    public string Address { get; set; }
    public DateTime ContractStartDate { get; set; }
    public decimal ContractAmount { get; set; } // Sözleşme Bedeli
    public bool IsApproved { get; set; } // Admin onayı (T6 gereksinimi için)

    // İlişkiler
    public List<FinancialReport> FinancialReports { get; set; }
}
