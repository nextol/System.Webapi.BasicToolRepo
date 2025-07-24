using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Webapi.BasicToolRepo.Contracts.InterFaces
{
    public interface IBasicToolLogger<T> where T : class
    {
        void Debug(string message);

        void Debug(Exception exception, string message);

        void Debug(string message, string objToLog);

        void Info(string message);

        void Info(Exception exception, string message);

        void Info(string message, params object[] templateParams);

        void Info(Exception exception, string message, params object[] templateParams);

        void Warn(Exception exception, string message);

        void Error(string message);

        void Error(string message, int Id);

        void Error(Exception exception, string message);

        Task<Tmodel> LogExecutionTimeAsync<Tmodel>(Func<Task<Tmodel>> operation, string operationName, string transactionId, string? extraInfo = null);
    }
}
