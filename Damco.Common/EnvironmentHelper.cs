using System;

namespace Damco.Common
{
    public static class EnvironmentHelper
    {
        public static bool IsAzure { get; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
    }
}
