using Microsoft.AspNetCore.Mvc;
using FintechPlatform.Models;
using FintechPlatform.Services;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace FintechPlatform.Controllers;

public class HomeController : Controller
{
    //private readonly HuggingFaceOcrService _ocrService;
    private readonly GeminiService _geminiService; // Bunu eklemelisin

    public HomeController(GeminiService geminiService)
    {
       // _ocrService = ocrService;
        _geminiService = geminiService; // Atamayý yapmalýsýn
    }

    public IActionResult Index() => View(new OcrViewModel());

    [HttpPost]
    public async Task<IActionResult> ProcessOcr(OcrViewModel model)
    {
        if (model.UploadedFile != null)
        {
            using var ms = new MemoryStream();
            await model.UploadedFile.CopyToAsync(ms);
            string base64 = Convert.ToBase64String(ms.ToArray());

            // Gemini servisini çaðýr
            string jsonResult = await _geminiService.ProcessDocumentAsync(base64, model.UploadedFile.ContentType);

            // Gelen JSON'u modele otomatik eþle
            var extractedData = JsonSerializer.Deserialize<GeminiResponse>(jsonResult);

            if (extractedData != null)
            {
                model.SirketUnvani = extractedData.sirketUnvani; // Örn: BÝOS MAKÝNA
                model.VergiNumarasi = extractedData.vergiNumarasi; // Örn: 1760430587[cite: 2]
                model.TicaretSicilNo = extractedData.ticaretSicilNo;
                model.KurulusTarihi = extractedData.kurulusTarihi;
                model.VergiMatrahi = extractedData.vergiMatrahi; // Örn: 2.514.347,66[cite: 2]
                model.YillikCiro = extractedData.yillikCiro;
            }
            model.OcrResult = jsonResult;
        }
        return View("Index", model);
    }

    // JSON verisini karþýlamak için geçici bir sýnýf (Controller içinde en alta ekleyebilirsin)
    public class GeminiResponse
    {
        public string sirketUnvani { get; set; }
        public string vergiNumarasi { get; set; }
        public string ticaretSicilNo { get; set; }
        public string kurulusTarihi { get; set; }
        public string faaliyetAlani { get; set; }
        public string yetkiliKisi { get; set; }
        public string yillikCiro { get; set; }
        public string vergiMatrahi { get; set; }
        public string ticariKar { get; set; }
        public string kkeg { get; set; }
    }
}