using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FintechPlatform.Models;
using FintechPlatform.Services;
using System.Text.RegularExpressions; // Regex için gerekli

namespace FintechPlatform.Controllers;

public class HomeController : Controller
{
    private readonly HuggingFaceOcrService _ocrService;

    public HomeController(HuggingFaceOcrService ocrService)
    {
        _ocrService = ocrService;
    }

    public IActionResult Index()
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
            var fileBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            string contentType = model.UploadedFile.ContentType;

            // 1. AI Servisinden ham metni al
            string result = await _ocrService.ProcessFileAsync(base64String, contentType);
            model.OcrResult = result;

            // 2. Ham metin içinden verileri Regex ile ayýkla (Parsing)
            // Beyanname formatýna göre sayýlarý yakalar (Örn: 1.920.200,90)
            model.TicariKar = ExtractValue(result, "Ticari Bilanço Karý");
            model.KKEG = ExtractValue(result, "Kanunen Kabul Edilmeyen Gider");
            model.VergiMatrahi = ExtractValue(result, "Geçici Vergi Matrahý");
        }
        return View("Index", model);
    }

    // Yardýmcý Metot: Metin içinden etiket ismine göre sayýsal deðeri çeker
    private string ExtractValue(string text, string fieldName)
    {
        if (string.IsNullOrEmpty(text)) return "0,00";

        // Regex: Alan adýndan sonra gelen boru (|) karakterini ve yanýndaki sayýyý yakalar
        string pattern = $@"{fieldName}\s*\|\s*([\d\.,]+)";
        var match = Regex.Match(text, pattern);

        return match.Success ? match.Groups[1].Value : "0,00";
    }
}