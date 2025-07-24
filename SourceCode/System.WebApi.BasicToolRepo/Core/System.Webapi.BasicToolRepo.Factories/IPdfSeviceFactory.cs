using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;

namespace System.Webapi.BasicToolRepo.Factories
{
    public interface IPdfSeviceFactory
    {
        IPdfGenerator CreatePdfGeneratorRepository();
    }
}
