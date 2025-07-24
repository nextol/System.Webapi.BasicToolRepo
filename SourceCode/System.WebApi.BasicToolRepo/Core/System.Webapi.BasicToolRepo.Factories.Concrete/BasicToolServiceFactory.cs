

using System.Net.Http;
using System.Webapi.BasicToolRepo.Entities.Models;
using System.Webapi.BasicToolRepo.Infrastructure;   
using System.Webapi.BasicToolRepo.Entities.RequestContext;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using System.Webapi.BasicToolRepo.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace System.Webapi.BasicToolRepo.Factories.Concrete
{
    public class BasicToolServiceFactory : IBasicToolImageServiceFactory
    {
        private readonly IBasicToolLogger<BasicToolRepository> _logger;
        private readonly IRequestContextInfo _gceContext;
        public BasicToolServiceFactory(IBasicToolLogger<BasicToolRepository> logger, IRequestContextInfo gceContext) 
        {
            _logger = logger;
            _gceContext = gceContext;
        }
        public virtual IBasicToolRepository CreateBasicToolRepository()
        {
            return new BasicToolRepository(_logger,
                             _gceContext);
        }
        public virtual IIgenericValidationRepository CreateValidationRepository()
        {
            return new GenericValidationRepository();
        }
    }
}
