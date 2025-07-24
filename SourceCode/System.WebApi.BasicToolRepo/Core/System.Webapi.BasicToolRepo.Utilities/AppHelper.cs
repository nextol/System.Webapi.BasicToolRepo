using NpgsqlTypes;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
namespace System.Webapi.BasicToolRepo.Utilities
{
    /// <summary>
    /// Added for supporting application helper methods.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AppHelper
    {
        private const int MaxWidth = 1024;
        private const int MaxHeight = 768;
        /// <summary>
        /// Enum for array data type options.
        /// </summary>
        public enum ArrayDataTypeOptions
        {
            Varchar = NpgsqlDbType.Array | NpgsqlDbType.Varchar,
            Bigint = NpgsqlDbType.Array | NpgsqlDbType.Bigint
        }

        /// <summary>
        /// Gets the SQL query text from an embedded resource file.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <param name="gdsSchema"></param>
        /// <param name="gppSchema"></param>
        /// <returns></returns>
        public static string GetSqlQueryText(Assembly assembly, string folderName, string fileName, string gdsSchema, string gppSchema)
        {
            if (assembly == null)
            {
                return string.Empty;
            }

            var resourcePath = $"System.GCE.WebApi.CostIntegrator.Utilities.DBQueryScripts.{folderName}.{fileName}.sql";

            using (Stream? stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    return string.Empty;
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(result))
                    {
                        return string.Empty;
                    }

                    return result.Replace("$SCHEMA_GCE$", gdsSchema)
                                 .Replace("$SCHEMA_GPP$", gppSchema);
                }
            }
        }

        /// <summary>
        /// Converts a DataTable to a list of objects of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToListof<T>(DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var objectProperties = typeof(T).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();
                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                }
                return instanceOfT;
            }).ToList();
            return targetList;
        }

        /// <summary>
        /// Checking dataset null or empty.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public static bool IsDataSetNullOrEmpty(DataSet dataSet, int tableIndex)
        {
            return dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[tableIndex].Rows.Count == 0;
        }

        /// <summary>
        /// Converts a single object into a DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clsObject"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(T clsObject)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = props[i].GetValue(clsObject) ?? DBNull.Value;
            }
            table.Rows.Add(values);
            return table;
        }

        /// <summary>
        /// Converts the specified <see cref="DateTime"/> to an ISO 8601 string in UTC format ("yyyy-MM-ddTHH:mm:ssZ").
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToIsoString(this DateTime date) => date.ToString("yyyy-MM-ddTHH:mm:ssZ");

        /// <summary>
        /// Converts a DateTime to UTC and sets the time to the start of the day (00:00:00).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToUtcStartOfDay(this DateTime date) => new(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a DateTime to UTC and sets the time to the end of the day (23:59:59).
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToUtcEndOfDay(this DateTime date) => new(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Utc);

        /// <summary>
        /// Converts a DataTable to a tilde-delimited string.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string DataTableToTildeString(DataTable dataTable)
        {
            var sb = new StringBuilder(dataTable.Rows.Count * 128);

            // Header
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                sb.Append(dataTable.Columns[i].ColumnName);
                if (i < dataTable.Columns.Count - 1)
                    sb.Append('~');
            }
            sb.AppendLine();

            // Rows
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.Append(row[i]?.ToString()?.Replace("~", " ") ?? string.Empty);
                    if (i < dataTable.Columns.Count - 1)
                        sb.Append('~');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a zip file from the specified file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="zipFilePath"></param>
        /// <param name="entryName"></param>
        public static void CreateZipFromFile(string filePath, string zipFilePath, string entryName)
        {
            using var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
            zip.CreateEntryFromFile(filePath, entryName, CompressionLevel.Optimal);
        }

        /// <summary>
        /// Deletes a file if it exists.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void TryDeleteFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}", path);

            File.Delete(path);
        }

        /// <summary>
        /// Checks if a directory exists on the SFTP server.
        /// </summary>
        /// <param name="sftp"></param>
        /// <param name="path"></param>
        /// <returns></returns>


        /// <summary>
        /// Converts a DataTable header to a tilde-delimited string.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string DataTableHeaderToTildeString(DataTable dataTable)
        {
            return string.Join("~", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
        }

        /// <summary>
        /// Converts a DataRow to a tilde-delimited string.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string DataRowToTildeString(DataRow row)
        {
            return string.Join("~", row.ItemArray.Select(item => item?.ToString() ?? string.Empty));
        }

        /// <summary>
        /// Parses the SFTP port from a string, returning 22 if parsing fails or if the string is null or empty.
        /// </summary>
        /// <param name="portString"></param>
        /// <param name="methodName"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static int ParseSftpPort(string portString, string methodName, string? transactionId)
        {
            int port = 22;
            if (!string.IsNullOrWhiteSpace(portString) && !int.TryParse(portString, out port))
            {
                port = 22;
            }
            return port;
        }

        /// <summary>
        /// Gets the list of zip files for today from the remote files collection.
        /// </summary>
        /// <param name="remoteFiles"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static List<dynamic> GetTodayZipFiles(IEnumerable<dynamic> remoteFiles, DateTime today)
        {
            var todayString = today.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            var todayFiles = new List<dynamic>();
            foreach (var f in remoteFiles)
            {
                if (!f.IsDirectory && f.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && f.Name.Contains(todayString, StringComparison.Ordinal))
                {
                    todayFiles.Add(f);
                }
            }
            return todayFiles;
        }
        /// <summary>
        /// Generic utility method to build an endpoint with query string parameters from any request object.
        /// </summary>
        /// <param name="baseEndpoint"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string BuildEndpointWithQuery<T>(string baseEndpoint, T request)
    where T : class
        {
            if (request == null)
                return baseEndpoint;

            var properties = typeof(T).GetProperties();
            var queryParams = new List<string>();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(request);
                if (ShouldSkipProperty(prop.PropertyType, value))
                    continue;

                // Ensure 'value' is not null before passing it to BuildQueryParam
                if (value != null)
                {
                    queryParams.Add(BuildQueryParam(prop, value));
                }
            }

            if (queryParams.Count == 0)
                return baseEndpoint;

            return $"{baseEndpoint}?{string.Join("&", queryParams)}";
        }

        private static bool ShouldSkipProperty(Type propertyType, object? value)
        {
            if (value == null)
                return true;

            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                return (DateTime)value == default;

            if (propertyType == typeof(bool))
                return !(bool)value;

            if (propertyType == typeof(string))
                return string.IsNullOrWhiteSpace((string)value);

            return false;
        }

        private static string BuildQueryParam(PropertyInfo prop, object value)
        {
            if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                var dateValue = (DateTime)value;
                return $"{prop.Name.ToLowerInvariant()}={Uri.EscapeDataString(dateValue.ToString("o"))}";
            }

            var stringValue = value?.ToString() ?? string.Empty; // Ensure stringValue is not null  
            return $"{prop.Name.ToLowerInvariant()}={Uri.EscapeDataString(stringValue)}";
        }

        public static void ResizeImageIfNeeded(Image image)
        {
            if (image.Width > MaxWidth || image.Height > MaxHeight)
            {
                image.Mutate(x =>
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(MaxWidth, MaxHeight),
                        Mode = ResizeMode.Max
                    }));
            }
        }

    }
}
