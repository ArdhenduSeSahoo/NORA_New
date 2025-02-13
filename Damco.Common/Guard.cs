using System;

namespace Damco.Common
{
    public static class Guard
    {
        public static void Against(string value, string exceptionMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(exceptionMessage);
            }
        }
    }
}
