using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Webapi.BasicToolRepo.Contracts;
using System.Webapi.BasicToolRepo.Entities.RequestContext;
using System.Webapi.BasicToolRepo.Utilities;
using static System.Webapi.BasicToolRepo.Utilities.Enums;

namespace System.Webapi.BasicToolRepo.Infrastructure
{
    /// <summary>
    /// Added for supporting PostgreSQL database operations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PostgresProviderRepository : IPostgresProviderRepository
    {
        #region Private/Public Fields
        private bool _disposed;
        private readonly IConfiguration _configuration;
        private readonly IRequestContextInfo _requestContext;
        #endregion

        public PostgresProviderRepository(IConfiguration configuration, IRequestContextInfo requestContext)
        {
            _configuration = configuration;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Added for getting the connection string based on the provided schema.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="isReadOnlyConnection"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string GetConnectionString(GceDbSchema schema, bool isReadOnlyConnection = false)
        {
            string connKeyPrimary = isReadOnlyConnection ? "gceAlloyDbConnectionBlueRO" : "gceAlloyDbConnectionBlue";
            string connKeySecondary = isReadOnlyConnection ? "gceAlloyDbConnectionGreenRO" : "gceAlloyDbConnectionGreen";

            string? primaryConnStr = _configuration.GetConnectionString(connKeyPrimary);
            if (string.IsNullOrWhiteSpace(primaryConnStr))
                throw new InvalidOperationException(Constants.ConnectionStringNotConfiguredMessage);

            var dbName = _requestContext.DatabaseName;
            if (string.IsNullOrWhiteSpace(dbName))
                throw new InvalidOperationException(Constants.DatabaseNameNotSetMessage);

            static string BuildConnStr(string baseConnStr, string dbName, GceDbSchema schema)
            {
                var builder = new NpgsqlConnectionStringBuilder(baseConnStr)
                {
                    Database = dbName,
                    SearchPath = schema.ToString()
                };
                return builder.ConnectionString;
            }

            string connStr = BuildConnStr(primaryConnStr, dbName, schema);

            try
            {
                using var primaryConn = new NpgsqlConnection(connStr);
                primaryConn.Open();
                return connStr;
            }
            catch
            {
                string? secondaryConnStr = _configuration.GetConnectionString(connKeySecondary);
                if (string.IsNullOrWhiteSpace(secondaryConnStr))
                    throw new InvalidOperationException(Constants.ConnectionStringNotConfiguredMessage);

                string secondaryConn = BuildConnStr(secondaryConnStr, dbName, schema);
                using var secondaryConnObj = new NpgsqlConnection(secondaryConn);
                secondaryConnObj.Open();
                return secondaryConn;
            }
        }

        /// <summary>
        /// Added for querying the database and returning a collection of results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="schema"></param>
        /// <param name="connection"></param>
        /// <param name="isReadOnlyConnection"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false)
        {
            if (connection is not null)
            {
                return await connection.QueryAsync<T>(sql, parameters).ConfigureAwait(false);
            }

            var connStr = GetConnectionString(schema, isReadOnlyConnection);
            await using var conn = new NpgsqlConnection(connStr);
            await conn.OpenAsync().ConfigureAwait(false);
            return await conn.QueryAsync<T>(sql, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Added for executing a SQL command asynchronously and returning the number of rows affected.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="schema"></param>
        /// <param name="connection"></param>
        /// <param name="isReadOnlyConnection"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false)
        {
            if (connection is not null)
                return await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            var connStr = GetConnectionString(schema, isReadOnlyConnection);
            await using var conn = new NpgsqlConnection(connStr);
            return await conn.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Added for querying the database and returns a single result or null if no result is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="schema"></param>
        /// <param name="connection"></param>
        /// <param name="isReadOnlyConnection"></param>
        /// <returns></returns>
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false)
        {
            if (connection is not null)
                return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters).ConfigureAwait(false);

            var connStr = GetConnectionString(schema, isReadOnlyConnection);
            await using var conn = new NpgsqlConnection(connStr);
            return await conn.QuerySingleOrDefaultAsync<T>(sql, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Added for querying the database and returns a DataTable containing the results.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="schema"></param>
        /// <param name="connection"></param>
        /// <param name="isReadOnlyConnection"></param>
        /// <returns></returns>
        public async Task<DataTable> QueryDataTableAsync(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false)
        {
            bool shouldDispose = false;
            NpgsqlConnection? localConnection = connection;
            if (localConnection is null)
            {
                localConnection = new NpgsqlConnection(GetConnectionString(schema, isReadOnlyConnection));
                await localConnection.OpenAsync().ConfigureAwait(false);
                shouldDispose = true;
            }

            try
            {
                using var command = new NpgsqlCommand(sql, localConnection);
                if (parameters is not null)
                {
                    var props = parameters.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(parameters) ?? DBNull.Value;
                        command.Parameters.AddWithValue(prop.Name, value);
                    }
                }

                using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess).ConfigureAwait(false);
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
            finally
            {
                if (shouldDispose)
                    await localConnection!.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Added for querying the database and returns a DataTable containing the results for large datasets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="schema"></param>
        /// <param name="connection"></param>
        /// <param name="isReadOnlyConnection"></param>
        /// <returns></returns>
        public async Task<DataTable> QueryLargeDataTableAsync<T>(string sql, object? parameters = null, GceDbSchema schema = GceDbSchema.gce, NpgsqlConnection? connection = null, bool isReadOnlyConnection = false)
        {
            bool shouldDispose = false;
            if (connection is null)
            {
                connection = new NpgsqlConnection(GetConnectionString(schema, isReadOnlyConnection));
                await connection.OpenAsync().ConfigureAwait(false);
                shouldDispose = true;
            }

            try
            {
                var results = await connection.QueryAsync<T>(sql, parameters).ConfigureAwait(false);

                var dataTable = new DataTable();
                var props = typeof(T).GetProperties();

                foreach (var prop in props)
                {
                    dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                foreach (var item in results)
                {
                    var values = props.Select(p => p.GetValue(item) ?? DBNull.Value).ToArray();
                    dataTable.Rows.Add(values);
                }

                return dataTable;
            }
            finally
            {
                if (shouldDispose)
                    await connection!.DisposeAsync().ConfigureAwait(false);
            }
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed resources here if needed in the future.
            }

            _disposed = true;
        }
        #endregion
    }
}
