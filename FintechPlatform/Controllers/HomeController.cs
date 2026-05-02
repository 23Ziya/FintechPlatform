using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FintechPlatform.Models;
using FintechPlatform.Services; // Servisimizi ekliyoruz
using Microsoft.AspNetCore.Http; // IFormFile için

namespace FintechPlatform.Controllers;

public class HomeController : Controller
{
    private readonly HuggingFaceOcrService _ocrService;

    // Servisi Constructor (Yapýcý Metot) ile içeri alýyoruz
    public HomeController(HuggingFaceOcrService ocrService)
    {
        _ocrService = ocrService;
    }

    // Sayfa ilk açýldýðýnda çalýþan yer (GET)
    public IActionResult Index()
    {
        return View(new OcrViewModel());
    }

    // Kullanýcý resim yükleyip butona bastýðýnda çalýþan yer (POST)
    [HttpPost]
    public async Task<IActionResult> ProcessOcr(OcrViewModel model)
    {
        if (model.UploadedFile != null && model.UploadedFile.Length > 0)
        {
            // 1. Dosyayý hafýzaya alýp Base64 formatýna çeviriyoruz
            using var ms = new MemoryStream();
            await model.UploadedFile.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);

            // 2. Servise gönderip OCR sonucunu bekliyoruz
            string result = await _ocrService.ProcessImageAsync(base64String);

            // 3. Sonucu View modeline atýyoruz
            model.OcrResult = result;
        }

        // Sonuçlarla birlikte ayný sayfayý (Index) tekrar yüklüyoruz
        return View("Index", model);
    }
}