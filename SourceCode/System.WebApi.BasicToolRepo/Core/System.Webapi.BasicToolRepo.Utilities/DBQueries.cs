using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Utilities
{
    /// <summary>
    /// Added for supporting application database queries.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DBQueries
    {
        public const string TAB_SBO_HEADER = "select sk_sbo, sk_sbo_group, sbo_nbr, sbo_version from tab_sbo_header where sk_company = @p_sk_company;";
    }
}
