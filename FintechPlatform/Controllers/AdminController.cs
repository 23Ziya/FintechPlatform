using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintechPlatform.Data;
using FintechPlatform.Models;

namespace FintechPlatform.Controllers
{
    // T1: Sadece Admin rolündekiler girebilir
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Paneli Ana Sayfası
        public IActionResult Index()
        {
            return View();
        }

        // T6: Bekleyen Abonelik Taleplerini Listele
        public async Task<IActionResult> Subscriptions()
        {
            var requests = await _context.SubscriptionRequests
                .Include(s => s.Company) // T10: N+1 problemini önlemek için Include kullanıyoruz
                .OrderByDescending(s => s.RequestDate)
                .ToListAsync();
            return View(requests);
        }

        // T6: Abonelik Talebini Onayla
        [HttpPost]
        public async Task<IActionResult> ApproveSubscription(int id)
        {
            var request = await _context.SubscriptionRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = "Onaylandı";

                // İlgili şirketin özelliklerini aktif et (Örn: IsPremium alanını güncelle)
                var company = await _context.Companies.FindAsync(request.CompanyId);
                if (company != null)
                {
                    // Şirket modelinde bu alanın olduğundan emin ol
                    // company.IsPremium = true; 
                }

                await _context.SaveChangesAsync();

                // T9: İşlemi logla
                _context.AuditLogs.Add(new AuditLog
                {
                    Action = $"Abonelik Onaylandı: Request ID {id}",
                    UserId = User.Identity.Name,
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Subscriptions));
        }

        // T9: Sistem Loglarını Görüntüle
        public async Task<IActionResult> Logs()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(100) // Performans için son 100 log
                .ToListAsync();
            return View(logs);
        }
    }
}