//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.ApplicationInsights;
//using System.Configuration;
//namespace Damco.Common
//{
//    public static class ApplicationInsights
//    {
//        /// <summary>
//        /// This method will create a new TelemetryClient to record messages to Application Insights
//        /// </summary>
//        /// <returns></returns>
//        public static TelemetryClient CreateTelemetryClient()
//        {
//            TelemetryClient telemetryClient = null;

//            // set the instrumentation key of App Insights to the telemetryClient
//            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"]))
//            { 
//            telemetryClient = new TelemetryClient();
//            telemetryClient.InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
//            }
            
//            return telemetryClient;
//        }
//    }
//}
