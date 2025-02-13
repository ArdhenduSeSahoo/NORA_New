
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceModel;
//using System.ServiceModel.Activation;
//using System.ServiceModel.Description;
//using System.Web;
//using Damco.Common;
//using System.ServiceModel.Dispatcher;
//using System.ServiceModel.Channels;
//using System.Collections.ObjectModel;
//using System.IdentityModel.Selectors;
//using System.Security;
//using System.Diagnostics;

//namespace Damco.Common
//{
//    /// <summary>
//    /// Utilitiy to take care of some basic setup for WCF services, so it does not have to be done
//    /// in the config file.
//    ///<para>To enable, add Factory="Damco.Common.BestPracticeServiceHostFactory" to the Tag %@ServiceHost 
//    /// directive at the top of the .svc file.</para>
//    /// </summary>
//    public static class WcfService
//    {
//        private static Type GetContractType(Type serviceType)
//        {
//            if (serviceType.GetCustomAttributes(typeof(ServiceContractAttribute), true).Any()) //Is a contract itself
//                return serviceType;
//            else //Find an interface
//            {
//                List<Type> contractTypes = serviceType.GetInterfaces()
//                    .Where(i => i.GetCustomAttributes(typeof(ServiceContractAttribute), true).Any())
//                    .ToList();
//                if (contractTypes.Count == 0)
//                    throw new ArgumentException($"{nameof(serviceType)} does not have the ServiceContract attribute and neither does any of it's interfaces; contractType cannot be determined");
//                else if (contractTypes.Count > 1)
//                    throw new ArgumentException($"{nameof(serviceType)} does not have the ServiceContract attribute and multiple of its interfaces do; contractType cannot be determined");
//                else
//                    return contractTypes.First();
//            }
//        }

//        static WcfServiceErrorHandler _errorHandler = new WcfServiceErrorHandler();

//        /// <summary>
//        /// Register the method to handle WCF related error.
//        /// </summary>
//        /// <param name="handler">Method which will handle WCF error.</param>
//        public static void RegisterErrorHandler(Action<Exception> handler)
//        {
//            _errorHandler.Handlers.Add(handler);
//        }

//        /// <summary>
//        /// Creates servicehost using the base address of hosted service,for service of type Tservice.
//        /// </summary>
//        /// <typeparam name="Tservice">Type of the Service.</typeparam>
//        /// <param name="url">Base address of the hosted service.</param>
//        /// <returns>Servicehost of type Tservice.</returns>
//        public static ServiceHost GetSimpleServiceHost<Tservice>(string url)
//        {
//            return GetSimpleServiceHost(typeof(Tservice), url);
//        }

//        /// <summary>
//        /// Creates servicehost for Singleton type service, using Base address of the service.
//        /// </summary>
//        /// <typeparam name="Tservice">Type of singleton service.</typeparam>
//        /// <param name="singletonInstance">Instance of singleton service.</param>
//        /// <param name="url">Base address of the hosted service.</param>
//        /// <returns></returns>
//        public static ServiceHost GetSimpleServiceHost<Tservice>(Tservice singletonInstance, string url)
//        {
//            return GetSimpleServiceHost(typeof(Tservice), (object)singletonInstance, url);
//        }

//        /// <summary>
//        /// Creates servicehost for input ServiceType of type Type, using Base address of the service.
//        /// </summary>
//        /// <param name="serviceType">Instance of service.</param>
//        /// <param name="url">Base address of the hosted service.</param>
//        /// <returns></returns>
//        public static ServiceHost GetSimpleServiceHost(Type serviceType, string url)
//        {
//            return GetSimpleServiceHost(serviceType, GetContractType(serviceType), default(object), url);
//        }


//        /// <summary>
//        /// Creates servicehost for input type either of ServiceType/singletonInstance. 
//        /// </summary>
//        /// <remarks>
//        ///  If singletonInstance is Null, then Servicehost for input type ServiceType is created.
//        /// </remarks>
//        /// <param name="serviceType"> Instance of service.</param>
//        /// <param name="singletonInstance">Instance of singleton service. </param>
//        /// <param name="url">Base address of the hosted service.</param>
//        /// <returns>Servicehost of type either of ServiceType/singletonInstance. </returns>
//        public static ServiceHost GetSimpleServiceHost(Type serviceType, object singletonInstance, string url)
//        {
//            return GetSimpleServiceHost(serviceType, GetContractType(serviceType), singletonInstance, url);
//        }

//        /// <summary>
//        /// Creates servicehost using the base address of hosted service,for Service of type Tservice and Contract of type Tcontract.
//        /// </summary>
//        /// <typeparam name="Tservice">Type of the Service.</typeparam>
//        /// <typeparam name="Tcontract"> Type of the Service Contract. </typeparam>
//        /// <param name="url">Base address of hosted service</param>
//        /// <returns>Servicehost of type Tcontract. </returns>
//        public static ServiceHost GetSimpleServiceHost<Tservice, Tcontract>(string url)
//            where Tservice : Tcontract
//        {
//            return GetSimpleServiceHost(typeof(Tservice), typeof(Tcontract), url);
//        }

//        /// <summary>
//        /// Creates servicehost for Singleton type service. 
//        /// </summary>
//        /// <typeparam name="Tservice">Type of the singleton service.</typeparam>
//        /// <typeparam name="Tcontract">Type of the service contract.</typeparam>
//        /// <param name="singletonInstance">Instance of singleton service.</param>
//        /// <param name="url">Base address of the hosted service. </param>
//        /// <returns>Servicehost of type Singleton instance.</returns>
//        public static ServiceHost GetSimpleServiceHost<Tservice, Tcontract>(Tservice singletonInstance, string url)
//            where Tservice : Tcontract
//        {
//            return GetSimpleServiceHost(typeof(Tservice), typeof(Tcontract), (object)singletonInstance, url);
//        }

//        /// <summary>
//        /// Creates servicehost for input ServiceType of type Type. 
//        /// </summary>
//        /// <param name="serviceType"> Instance of service.</param>
//        /// <param name="contractType"> Contract type of the service.</param>
//        /// <param name="url">Base address of the hosted service.</param>
//        /// <returns>Servicehost of type ServiceType. </returns>
//        public static ServiceHost GetSimpleServiceHost(Type serviceType, Type contractType, string url)
//        {
//            return GetSimpleServiceHost(serviceType, contractType, default(object), url);
//        }

//        /// <summary>
//        /// Creates servicehost for type either of ServiceType/singletonInstance. 
//        /// </summary>
//        /// <remarks>
//        ///  If singletonInstance is Null, then Servicehost of input type ServiceType is created.
//        /// </remarks>
//        /// <param name="serviceType"> Instance of service.</param>
//        /// <param name="contractType"> Contract type of the service.</param>
//        /// <param name="singletonInstance">Instance of singleton service. </param>
//        /// <param name="url">Base address of the hosted service.</param>
//        /// <returns>Servicehost of type either of ServiceType/singletonInstance. </returns>
//        public static ServiceHost GetSimpleServiceHost(Type serviceType, Type contractType, object singletonInstance, string url)
//        {
//            WcfParameters urlParams = WcfParameters.GetAndRemoveFromUrl(ref url);
//            url = WcfClient.GetRealUrl(url, contractType);

//            ServiceHost host;
//            if (singletonInstance == null)
//                host = new ServiceHost(serviceType, new Uri(url));
//            else
//                host = new ServiceHost(singletonInstance, new Uri(url));

//            host.AddDefaultEndpoints();
//            if (urlParams.RESTful)
//            {
//                host.Description.Endpoints[0].Binding = new System.ServiceModel.WebHttpBinding();
//                host.Description.Endpoints[0].EndpointBehaviors.Add(new WebHttpBehavior());
//            }

//            if (singletonInstance != null)
//                foreach (var serviceBehaviour in host.Description.Behaviors.OfType<ServiceBehaviorAttribute>())
//                    if (serviceBehaviour.InstanceContextMode != InstanceContextMode.Single)
//                        serviceBehaviour.InstanceContextMode = InstanceContextMode.Single;

//            host.SetBestPractices();

//            if (urlParams.Users.Length > 0)
//            {
//                if (!host.Description.Behaviors.OfType<ValidateAccessBehaviour>().Any())
//                    host.Description.Behaviors.Add(new ValidateAccessBehaviour(urlParams.Users, urlParams.Clients));
//                foreach (var endpoint in host.Description.Endpoints)
//                {
//                    if (endpoint.Binding is BasicHttpBinding)
//                    {
//                        var binding = (BasicHttpBinding)endpoint.Binding;
//                        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
//                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
//                    }
//                }
//            }
//            else if (urlParams.Clients.Length > 0)
//            {
//                if (!host.Description.Behaviors.OfType<ValidateAccessBehaviour>().Any())
//                    host.Description.Behaviors.Add(new ValidateAccessBehaviour(urlParams.Users, urlParams.Clients));
//            }

//            return host;
//        }


//        /// <summary>
//        /// Set the servicehost default parameters like Behaviors,Endpoint & Error handler.
//        /// </summary>
//        /// <param name="host">Servicehost for which parameters will be set.</param>
//        public static void SetBestPractices(this ServiceHostBase host)
//        {
//            //Metadata
//            if (host.Description.Endpoints.Any(e => e.Binding is HttpBindingBase || e.Binding is WebHttpBinding) &&
//                !host.Description.Behaviors.Any(b => b is ServiceMetadataBehavior))
//                host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });

//            //End points
//            if (host.Description.Endpoints.Count == 0)
//                host.AddDefaultEndpoints();

//            foreach (var endpoint in host.Description.Endpoints)
//            {
//                //Prevent tempuri.org
//                if (endpoint.Binding.Namespace == "http://tempuri.org/" && endpoint.Contract.Namespace != "http://tempuri.org/")
//                    endpoint.Binding.Namespace = endpoint.Contract.Namespace;
//                if (endpoint.Binding is BasicHttpBinding && ((BasicHttpBinding)endpoint.Binding).MaxReceivedMessageSize < 10485760)
//                    ((BasicHttpBinding)endpoint.Binding).MaxReceivedMessageSize = 10485760;
//            }

//            if (!host.Description.Behaviors.Contains(_errorHandler))
//                host.Description.Behaviors.Add(_errorHandler);
//        }

//    }

//    /// <summary>
//    /// Utility to create service host, depending upon the service class constructed by factory.
//    /// </summary>
//    public class BestPracticeServiceHostFactory : System.ServiceModel.Activation.ServiceHostFactory
//    {
//        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
//        {
//            var result = base.CreateServiceHost(constructorString, baseAddresses);
//            result.SetBestPractices();
//            return result;
//        }
//    }


//    /// <summary>
//    /// Utility to handle the WCF access related behaviour.
//    /// </summary>
//    /// <remarks> Implements IServiceBehavior, IDispatchMessageInspector for implementing default behaviors.</remarks>
//    internal class ValidateAccessBehaviour : IServiceBehavior, IDispatchMessageInspector
//    {
//        private WcfParameters.User[] Users;
//        private string[] Clients;

//        public ValidateAccessBehaviour(WcfParameters.User[] users, string[] clients)
//        {
//            this.Users = users;
//            this.Clients = clients;
//        }

//        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
//        {
//        }

//        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
//        {
//            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
//                foreach (EndpointDispatcher endpointDispatcher in dispatcher.Endpoints)
//                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
//        }


//        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
//        {
//        }

//        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
//        {
//            if (this.Users != null && this.Users.Any()
//                && (ServiceSecurityContext.Current != null && this.Users.Any(u => u.Name.Equals(ServiceSecurityContext.Current.PrimaryIdentity.Name, StringComparison.OrdinalIgnoreCase))))
//                return null;

//            if (this.Clients != null && this.Clients.Any())
//            {
//                var client = System.Net.Dns.GetHostEntry(((RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]).Address);
//                if (this.Clients.Any(c =>
//                    (c.Equals("localhost", StringComparison.OrdinalIgnoreCase) && client.HostName == System.Net.Dns.GetHostEntry("localhost").HostName)
//                    || (client.HostName != null && (client.HostName.Equals(c, StringComparison.OrdinalIgnoreCase) || client.HostName.StartsWith($"{c}.", StringComparison.OrdinalIgnoreCase)))
//                    || client.AddressList.Any(a => a.ToString() == c)))
//                    return null;
//            }

//            //If validated, this code is not reached

//            if (this.Users != null && this.Users.Any() & this.Clients != null && this.Clients.Any())
//                throw new FaultException($"User '{ServiceSecurityContext.Current?.PrimaryIdentity.Name ?? "<unknown>"}' nor client '{((RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]).Address}'  has access to this service.");
//            else if (this.Users != null && this.Users.Any())
//                throw new FaultException($"User '{ServiceSecurityContext.Current?.PrimaryIdentity.Name ?? "<unknown>"}' does not have access to this service.");
//            else if (this.Clients != null && this.Clients.Any())
//                throw new FaultException($"Client '{((RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]).Address}' does not have access to this service.");
//            else
//                return null; //no access control
//        }

//        public void BeforeSendReply(ref Message reply, object correlationState)
//        {
//        }
//    }

//    /// <summary>
//    /// Utility to handle the WCF related exceptions.
//    /// </summary>
//    /// <remarks> Implements IServiceBehavior, IErrorHandler for implementing default behaviors.</remarks>

//    internal class WcfServiceErrorHandler : IServiceBehavior, IErrorHandler
//    {
//        internal List<Action<Exception>> Handlers { get; set; } = new List<Action<Exception>>();

//        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
//        {
//        }

//        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
//        {
//            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
//                if (!dispatcher.ErrorHandlers.Contains(this))
//                    dispatcher.ErrorHandlers.Add(this);
//        }

//        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
//        {
//        }

//        public bool HandleError(Exception error)
//        {
//            foreach (var handler in this.Handlers)
//                handler(error);
//            return false;
//        }

//        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
//        {
//        }
//    }
    
//}