using Microsoft.AspNetCore.Mvc;
using FintechPlatform.Models;
using FintechPlatform.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace FintechPlatform.Controllers;
//[Authorize(Roles = "Admin")]

public class CompaniesController : Controller
{
    private readonly GeminiService _geminiService;

    public CompaniesController(GeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    // Admin'in yeni şirket ekleme ve AI analiz sayfası
    [HttpGet]
    public IActionResult Create()
    {
        return View(new OcrViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> ProcessOcr(OcrViewModel model)
    {
        if (model.UploadedFile != null && model.UploadedFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await model.UploadedFile.CopyToAsync(ms);
            string base64 = Convert.ToBase64String(ms.ToArray());

            // Gemini 1.5 Flash ile belge analizi[cite: 1]
            string jsonResult = await _geminiService.ProcessDocumentAsync(base64, model.UploadedFile.ContentType);
            model.OcrResult = jsonResult;

            if (!string.IsNullOrEmpty(jsonResult) && jsonResult.Trim().StartsWith("{") && !jsonResult.Contains("\"error\":"))
            {
                try
                {
                    var data = JsonSerializer.Deserialize<GeminiResponse>(jsonResult);
                    if (data != null)
                    {
                        // FADA ve BİOS[cite: 2] dökümanlarından gelen veriler
                        model.SirketUnvani = data.sirketUnvani;
                        model.VergiNumarasi = data.vergiNumarasi;
                        model.TicaretSicilNo = data.ticaretSicilNo;
                        model.KurulusTarihi = data.kurulusTarihi;
                        model.YetkiliKisi = data.yetkiliKisi;
                        model.YillikCiro = data.yillikCiro;
                    }
                }
                catch { /* Hata yönetimi */ }
            }
        }
        return View("Create", model);
    }

    public class GeminiResponse
    {
        public string? sirketUnvani { get; set; }
        public string? vergiNumarasi { get; set; }
        public string? ticaretSicilNo { get; set; }
        public string? kurulusTarihi { get; set; }
        public string? yetkiliKisi { get; set; }
        public string? yillikCiro { get; set; }
    }
}