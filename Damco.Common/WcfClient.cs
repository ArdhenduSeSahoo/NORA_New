//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.ServiceModel;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Description;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Web;

//namespace Damco.Common
//{

//    /// <summary>
//    /// Utilities to handle client WCF service.
//    /// </summary>
//    public class WcfClient
//    {
//        /// <summary>
//        /// Gets the actual base address of service hosted.
//        /// </summary>
//        /// <param name="url">Base address of service hosted. </param>
//        /// <param name="contractOrProxy">Contract/Proxy of the service.</param>
//        /// <returns>Base address of service hosted.</returns>
//        internal static string GetRealUrl(string url, Type contractOrProxy)
//        {
//            if (!url.Trim().ToLower().EndsWith(".svc")
//                && !url.Trim().ToLower().EndsWith(".asmx")
//                && !url.EndsWith("/")) //Service name missing - add it based on default settings
//            {
//                string strWebServiceName = contractOrProxy.Name;
//                if (contractOrProxy.IsInterface)
//                {
//                    if (strWebServiceName.StartsWith("I")) strWebServiceName = strWebServiceName.Substring(1);
//                }
//                else //Proxy
//                {
//                    if (strWebServiceName.EndsWith("Client")) strWebServiceName = strWebServiceName.Substring(0, strWebServiceName.Length - "Client".Length);
//                }
//                return $"{url}/{strWebServiceName}/";
//            }
//            else
//                return url;
//        }


//        /// <summary>
//        /// Creates client wcf service of specified type.
//        /// </summary>
//        /// <typeparam name="T">Type of the service.</typeparam>
//        /// <param name="url">Base address of the hosted service.</param>
//        /// <returns>WCF service.</returns>
//        public static T GetService<T>(string url)
//        {
//            return GetService<T>(url, false);
//        }
//        public static T GetService<T>(string url, bool useUrlExactly)
//        {
//            WcfParameters parameters = WcfParameters.GetAndRemoveFromUrl(ref url);

//            if (parameters.Users.Length > 1)
//                throw new ArgumentException("A client cannot have mulitple users setups.");

//            if (!useUrlExactly)
//                url = GetRealUrl(url, typeof(T));

//            Binding binding;

//            if (url.StartsWith("net.pipe://", StringComparison.OrdinalIgnoreCase))
//            {
//                var bin = new NetNamedPipeBinding();
//                binding = bin;
//                bin.CloseTimeout = TimeSpan.FromSeconds(10);
//                bin.OpenTimeout = TimeSpan.FromSeconds(10);
//                if (parameters.TimeOut != -1)
//                {
//                    bin.ReceiveTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                    bin.SendTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                }
//                bin.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
//                bin.MaxBufferSize = 2147483647;
//                bin.MaxBufferPoolSize = 524288;
//                bin.MaxReceivedMessageSize = 2147483647;
//                bin.TransferMode = TransferMode.Buffered;
//            }

//            else if (url.StartsWith("net.tcp://", StringComparison.OrdinalIgnoreCase))
//            {
//                var bin = new NetTcpBinding();
//                binding = bin;
//                bin.CloseTimeout = TimeSpan.FromSeconds(10);
//                bin.OpenTimeout = TimeSpan.FromSeconds(10);
//                if (parameters.TimeOut != -1)
//                {
//                    bin.ReceiveTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                    bin.SendTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                }
//                bin.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
//                bin.MaxBufferSize = 2147483647;
//                bin.MaxBufferPoolSize = 524288;
//                bin.MaxReceivedMessageSize = 2147483647;
//                bin.TransferMode = TransferMode.Buffered;
//            }

//            else if (parameters.RESTful)
//            {
//                var bin = new WebHttpBinding();
//                binding = bin;
//                bin.CloseTimeout = TimeSpan.FromSeconds(10);
//                bin.OpenTimeout = TimeSpan.FromSeconds(10);
//                if (parameters.TimeOut != -1)
//                {
//                    bin.ReceiveTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                    bin.SendTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                }
//                bin.AllowCookies = false;
//                bin.BypassProxyOnLocal = false;
//                bin.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
//                bin.MaxBufferSize = 2147483647;
//                bin.MaxBufferPoolSize = 524288;
//                bin.MaxReceivedMessageSize = 2147483647;
//                bin.TransferMode = TransferMode.Buffered;
//                bin.UseDefaultWebProxy = true;
//                bin.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
//                bin.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
//                bin.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
//                bin.Security.Transport.Realm = "";
//            }

//            else //http
//            {
//                var bin = new BasicHttpBinding();
//                binding = bin;
//                bin.CloseTimeout = TimeSpan.FromSeconds(10);
//                bin.OpenTimeout = TimeSpan.FromSeconds(10);
//                if (parameters.TimeOut != -1)
//                {
//                    bin.ReceiveTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                    bin.SendTimeout = TimeSpan.FromMilliseconds(parameters.TimeOut);
//                }
//                bin.AllowCookies = false;
//                bin.BypassProxyOnLocal = false;
//                bin.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
//                bin.MaxBufferSize = 2147483647;
//                bin.MaxBufferPoolSize = 524288;
//                bin.MaxReceivedMessageSize = 2147483647;
//                bin.MessageEncoding = WSMessageEncoding.Text;
//                bin.TextEncoding = Encoding.UTF8;
//                bin.TransferMode = TransferMode.Buffered;
//                bin.UseDefaultWebProxy = true;
//                bin.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
//                bin.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
//                bin.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
//                bin.Security.Transport.Realm = "";
//                bin.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
//            }

//            if (typeof(T).IsInterface) //Known contract
//            {
//                var channelFactory = new ChannelFactory<T>(binding, new EndpointAddress(url));
//                if (binding is WebHttpBinding)
//                    channelFactory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
//                if (parameters.Users.Length == 0)
//                    channelFactory.Credentials.Windows.ClientCredential = (NetworkCredential)System.Net.CredentialCache.DefaultCredentials;
//                else
//                    channelFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential(parameters.Users.First().Name, parameters.Users.First().Password);
//                return channelFactory.CreateChannel();
//            }
//            else if (
//                typeof(T).BaseType != null &&
//                typeof(T).BaseType.IsGenericType &&
//                typeof(T).BaseType.GetGenericTypeDefinition() == typeof(System.ServiceModel.ClientBase<>))
//            {
//                T tResult = (T)typeof(T).GetConstructor(new Type[] { typeof(System.ServiceModel.Channels.Binding), typeof(EndpointAddress) }).Invoke(new object[] { binding, new EndpointAddress(url) });
//                if (parameters.Users.Length == 0)
//                    ((ClientCredentials)typeof(T).GetProperty("ClientCredentials").GetValue(tResult, null)).Windows.ClientCredential = (NetworkCredential)System.Net.CredentialCache.DefaultCredentials;
//                else
//                    ((ClientCredentials)typeof(T).GetProperty("ClientCredentials").GetValue(tResult, null)).Windows.ClientCredential = new System.Net.NetworkCredential(parameters.Users.First().Name, parameters.Users.First().Password);
//                if (binding is WebHttpBinding)
//                    ((ChannelFactory)typeof(T).GetProperty("ChannelFactory").GetValue(tResult, null))
//                        .Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
//                return tResult;
//            }
//            else
//                throw new ArgumentException("T must be a contract interface or derived from System.ServiceModel.ClientBase<> (as proxy-generated <servicename>Client classes are)");
//        }
//    }
//}
