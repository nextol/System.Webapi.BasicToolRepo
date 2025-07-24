using Npgsql;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using static System.Webapi.BasicToolRepo.Utilities.Enums;

namespace System.Webapi.BasicToolRepo.Contracts
{

    /// <summary>
    /// Added for supporting postgres operations.
    /// </summary>
    public interface IPostgresProviderRepository : IDisposable
    {
        string GetConnectionString(GceDbSchema schema, bool isReadOnlyConnection = false);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false);

        Task<int> ExecuteAsync(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false);

        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false);

        Task<DataTable> QueryDataTableAsync(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false);

        Task<DataTable> QueryLargeDataTableAsync<T>(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false);
    }
}
