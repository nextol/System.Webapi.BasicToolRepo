using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo.Entities.PdfRepo;
using Microsoft.AspNetCore.Http;

namespace System.Webapi.BasicToolRepo.Contracts.InterFaces
{
    public interface IPdfGenerator: IDisposable
    {
        Task<MemoryStream> GeneratePdfFromImagesAsync(PdfRequest request);
    }
}
