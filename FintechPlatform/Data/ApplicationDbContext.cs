using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using FintechPlatform.Models;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Company> Companies { get; set; }
    public DbSet<FinancialReport> FinancialReports { get; set; }
    public DbSet<SubscriptionRequest> SubscriptionRequests { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // T10: Veritabanı optimizasyonu için Vergi Numarasına Index ekle
        builder.Entity<Company>().HasIndex(c => c.TaxNumber).IsUnique();
    }
}
