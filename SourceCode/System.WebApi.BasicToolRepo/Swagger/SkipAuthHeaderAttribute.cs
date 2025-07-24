using System;
using System.Diagnostics.CodeAnalysis;

namespace System.WebApi.BasicToolRepo.Swagger
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SkipAuthHeaderAttribute : Attribute
    {
    }
}
