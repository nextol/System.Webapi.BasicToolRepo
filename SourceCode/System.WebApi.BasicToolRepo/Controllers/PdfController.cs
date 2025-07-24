using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using System.Webapi.BasicToolRepo.Entities.PdfRepo;
using System.Webapi.BasicToolRepo.Factories;

namespace System.Webapi.BasicToolRepo.Controllers
{
    [Route("Basictool/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IPdfToolServiceFactory _pdfToolServiceFactory;

        public PdfController(IPdfToolServiceFactory pdfToolServiceFactory)
        {
            _pdfToolServiceFactory = pdfToolServiceFactory;
        }

        [HttpPost("GenerateCombinedPdf")]
        public async Task<IActionResult> GeneratePdf([FromForm] PdfRequest request)
        {
            if (request.Files == null || request.Files.Count == 0)
                return BadRequest("No files uploaded.");
            using var _pdfGenratorRepo = _pdfToolServiceFactory.CreatePdfGeneratorRepository();
            var pdfStream = await _pdfGenratorRepo.GeneratePdfFromImagesAsync(request);
            var fileName = $"combined_{DateTime.UtcNow:yyyyMMddHHmmssfff}.pdf";
            return File(pdfStream, "application/pdf", fileName);
        }
    }
}
