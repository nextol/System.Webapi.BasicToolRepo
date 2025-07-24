using Microsoft.AspNetCore.Http;
using System.Webapi.BasicToolRepo.Logging;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using System.Webapi.BasicToolRepo.Entities.PdfRepo;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Webapi.BasicToolRepo.Utilities;

namespace System.Webapi.BasicToolRepo.Infrastructure.Repositories
{
    public class PdfGenerator : IPdfGenerator
    {
        private readonly IBasicToolLogger<PdfGenerator> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int JpegQuality = 75;
        public PdfGenerator(IBasicToolLogger<PdfGenerator> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MemoryStream> GeneratePdfFromImagesAsync(PdfRequest request)
        {
            var requestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
            try
            {
                if (request.Files == null || request.Files.Count == 0)
                {
                    _logger.Info("RequestId: {RequestId} - No files received", templateParams: requestId);
                    throw new ArgumentException("No files provided.");
                }

                _logger.Info("RequestId: {RequestId} - Received {FileCount} files",
                             requestId,
                             request.Files.Count);

                using var document = new PdfDocument();

                int index = 1;
                foreach (var file in request.Files)
                {
                    _logger.Info("RequestId: {RequestId} - Processing image #{Index}: {FileName}", requestId, index++, file.FileName);
                    await AddImageToPdfDocumentAsync(document, file,index);
                }

                _logger.Info("RequestId: {RequestId} - PDF generation completed with {PageCount} pages", requestId, document.PageCount);

                var pdfStream = new MemoryStream();
                document.Save(pdfStream);
                pdfStream.Position = 0;
                return pdfStream;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"RequestId: {requestId} - An error occurred while generating PDF");
                throw; // Re-throwing to preserve stack trace
            }
        }
        #region Helper Method
        private static async Task AddImageToPdfDocumentAsync(PdfDocument document, IFormFile file, int index)
        {
            await using var imageStream = file.OpenReadStream();

            // ✅ Correct usage for ImageSharp 3.1.10
            using var image = await Image.LoadAsync(imageStream); // No generic, no out parameter

            AppHelper.ResizeImageIfNeeded(image);

            var page = document.AddPage();
            page.Width = XUnit.FromPoint(image.Width);
            page.Height = XUnit.FromPoint(image.Height);

            using var gfx = XGraphics.FromPdfPage(page);

            using var ms = new MemoryStream();
            await image.SaveAsJpegAsync(ms, new JpegEncoder { Quality = JpegQuality });  // ✅ This still works
            ms.Position = 0;

            using var xImage = XImage.FromStream(() => ms);
            gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);

            if (index % 100 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        #endregion
        #region IDisposable Patteren 
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects) if needed
                    // No managed resources to dispose in this class as of now
                    // If any IDisposable fields are added in the future, dispose them here
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GenericValidationRepository() // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
