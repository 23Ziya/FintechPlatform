using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FintechPlatform.Data;
using FintechPlatform.Models;
using System.Security.Claims;

namespace FintechPlatform.Controllers
{
    // Sadece giriş yapmış "User" rolündekiler girebilir
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kullanıcı Paneli Ana Sayfa 
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Kullanıcının bağlı olduğu şirketi getiriyoruz
            // Not: User - Company ilişkisini IdentityUser üzerinden veya bir tabloyla kurmuş olmalısın
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Name == User.Identity.Name); // Basit eşleştirme

            return View(company);
        }

        //Premium Paket Talebi Oluşturma
        [HttpPost]
        public async Task<IActionResult> RequestPremium(string packageType, int companyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = new SubscriptionRequest
            {
                CompanyId = companyId,
                UserId = userId,
                PackageType = packageType,
                RequestDate = DateTime.Now,
                Status = "Beklemede"
            };

            _context.SubscriptionRequests.Add(request);

            //İşlemi logla
            _context.AuditLogs.Add(new AuditLog
            {
                Action = $"Premium Talebi Oluşturuldu: {packageType}",
                UserId = User.Identity.Name,
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}