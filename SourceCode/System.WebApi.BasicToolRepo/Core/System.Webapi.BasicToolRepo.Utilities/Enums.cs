using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Utilities
{
    /// <summary>
    /// Added for supporting application enumerations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Enums
    {
        public enum GceDbSchema
        {
            gce,
            gde,
            gpp,
            none
        }
        public enum SystemApiMethods
        {
            CreateCostProgram,
            UpdateCostProgram,
            GetCostProgramDetail,
            ListCostPrograms,
            UpdateCostProgramStatus,
            AvailableCostProgram
        }
        public enum SystemApiControllers
        {
            CostProgram
        }
        public enum HttpRequestStatus
        {
            SUCCESS = 200,
            FAIL,
            FAILED = 202,
            NOTFOUND = 404,
            PARTIALFAILURE
        }
    }
}
