using System.Webapi.BasicToolRepo.Entities.RequestContext;

namespace System.Webapi.BasicToolRepo.Contracts.InterFaces
{
    /// <summary>
    /// Added for supporting request context operations.
    /// </summary>
    public interface IRequestContextProvider
    {
        RequestContext GetCurrentContext();
    }
}
