using System.Net.Http;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;

namespace System.Webapi.BasicToolRepo.Factories
{

    /// <summary>
    /// Abstract factory for all infrstructure services 
    ///     that are required by the core project
    /// </summary>
    public interface IBasicToolServiceFactory

    {
        IBasicToolRepository CreateBasicToolRepository();

    }
}
