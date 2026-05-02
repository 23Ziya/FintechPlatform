using System;

public class FinancialReport
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }

    public decimal BalanceSheetTotal { get; set; } // Bilanço Özeti
    public decimal NetProfit { get; set; } // Kar/Zarar
    public string BankDataJson { get; set; } // Banka verileri (Json olarak tutmak MVP için hızlıdır)

    public string AiAnalysisReport { get; set; } // LLM tarafından üretilen rapor (T4 gereksinimi)
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}