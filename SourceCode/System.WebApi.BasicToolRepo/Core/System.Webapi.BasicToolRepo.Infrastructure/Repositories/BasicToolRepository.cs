

using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using System.Webapi.BasicToolRepo.Entities.RequestContext;


namespace System.Webapi.BasicToolRepo.Infrastructure.Repositories
{
    public class BasicToolRepository : IBasicToolRepository
    {
        private readonly IBasicToolLogger<BasicToolRepository> _logger;
        private readonly IRequestContextInfo _gceContext;
        public BasicToolRepository(IBasicToolLogger<BasicToolRepository> logger, IRequestContextInfo gceContext)
        {
            _logger = logger;
            _gceContext = gceContext;
        }
      

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
        // ~CostManagementBaseRepository() // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
