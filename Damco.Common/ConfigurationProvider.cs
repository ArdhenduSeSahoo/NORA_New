using System;
using System.Configuration;

namespace Damco.Common
{
    public static class ConfigurationProvider
    {
        public static string Get(string key) => 
            Environment.GetEnvironmentVariable("APPSETTING_" + key)
                .IfNullOrEmptyTake(ConfigurationManager.AppSettings[key])
                .IfNullOrEmptyTake(Environment.GetEnvironmentVariable(key));

        private static string IfNullOrEmptyTake(this string input, string alternative)
        {
            return string.IsNullOrEmpty(input) ? alternative : input;
        }
        //all used configuration keys should be listed here
        public const string InstanceType = "InstanceType";
        public const string ServiceLocation = "ServiceLocation";
        public const string ClientId = "ida:ClientId";
        public const string Authority = "ida:Authority";
        public const string Domain = "ida:Domain";
        public const string BackgroundProcessorSecondChanceSchedule = "BackgroundProcessorSecondChanceSchedule";
        public const string MaxiumumConcurrentBackgroundProcesses = "MaxiumumConcurrentBackgroundProcesses";
        public const string InstanceId = "InstanceId";
        public const string SendEmails = "SendEmails";
        public const string FrontEndRootUrl = "FrontEndRootUrl";
        public const string DoOperationSchedules = "DoOperationSchedules";
        public const string APPINSIGHTS_INSTRUMENTATIONKEY = "APPINSIGHTS_INSTRUMENTATIONKEY";
        public const string MixedAuthentication = "MixedAuthentication";
        public const string BlobStorageConnectionString = "BlobStorageConnectionString";
        public const string KeyVaultUriSettingKey = "KeyVaultUri";
        public const string SqlConnectionString =  "SqlConnectionString";
        //need to replace all Environment.GetEnvironmentVariable and ConfigurationManager.AppSettings
    }
}
