using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damco.Common;

namespace Damco.Common
{
    /// <summary>
    /// Utilities to handle exception details.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Gets the Exception details.
        /// </summary>
        /// <remarks>
        /// Internally it use ObjectExtensions method.
        /// </remarks>
        /// <param name="exception">Exception whose details are required.</param>
        /// <returns>Details of exception.</returns>
        public static string ToStringWithFullDetails(this Exception exception)
        {
            return exception.FlattenTree(e => e.InnerException)
                .Reverse()
                .Select(e => e.ToStringWithFullDetailsNoInnerExceptions())
                .JoinStrings("\n\n");
        }

        private static string ToStringWithFullDetailsNoInnerExceptions(this Exception exception)
        {
            StringBuilder result = new StringBuilder(exception.ToString());
            foreach (var key in exception.Data.Keys)
                result.Append($"\n{key}: {exception.Data[key]}");
            return result.ToString();
        }


        /// <summary>
        /// Gets the Exception details with specified length.
        /// </summary>
        /// <param name="exception">Exception whose details are required.</param>
        /// <param name="maxLength">Allowed length of exception details.</param>
        /// <returns>Details of exception with specified length.</returns>
        public static string ToStringWithFullDetails(this Exception exception, int maxLength)
        {
            var result = exception.ToStringWithFullDetails();
            if (result.Length > maxLength)
                return result.Substring(0, maxLength);
            else
                return result;
        }

        public static Exception GetInnerMostException(this Exception exception)
        {
            while (exception.InnerException != null)
                exception = exception.InnerException;
            return exception;
        }
    }
}
