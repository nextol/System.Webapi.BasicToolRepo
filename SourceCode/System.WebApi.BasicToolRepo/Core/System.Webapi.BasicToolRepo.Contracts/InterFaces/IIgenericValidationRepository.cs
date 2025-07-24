
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Webapi.BasicToolRepo.Entities.Validation;

namespace System.Webapi.BasicToolRepo.Contracts.InterFaces
{
    public interface IIgenericValidationRepository:IDisposable
    {
        ValidationResponse IsValidRequest<TModel>(TModel instance, bool validateAllProperties = true)
            where TModel : class;

    }
}
