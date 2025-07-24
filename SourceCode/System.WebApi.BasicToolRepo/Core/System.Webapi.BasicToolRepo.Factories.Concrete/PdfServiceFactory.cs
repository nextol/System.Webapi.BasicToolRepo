using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using System.Webapi.BasicToolRepo.Entities.RequestContext;
using System.Webapi.BasicToolRepo.Infrastructure.Repositories;

namespace System.Webapi.BasicToolRepo.Factories.Concrete
{
    public class PdfServiceFactory: IPdfToolServiceFactory
    {
        private readonly IBasicToolLogger<PdfGenerator> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PdfServiceFactory(IBasicToolLogger<PdfGenerator> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public virtual IPdfGenerator CreatePdfGeneratorRepository()
        {
            return new PdfGenerator(_logger,
                             _httpContextAccessor);
        }
        public virtual IIgenericValidationRepository CreateValidationRepository()
        {
            return new GenericValidationRepository();
        }
    }
}
