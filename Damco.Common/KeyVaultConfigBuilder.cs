//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using Microsoft.Configuration.ConfigurationBuilders;


//namespace Damco.Common
//{
//    public class KeyVaultConfigBuilder : KeyValueConfigBuilder
//    {
//        private static readonly IReadOnlyCollection<string> _allowedKeys = new HashSet<string>(new[]
//        {
//            "NORA",
//            "NORAReport"
//        });

//        private KeyVaultService _keyVaultService;
//        public override void Initialize(string name, NameValueCollection config)
//        {
//            base.Initialize(name, config);
//            _keyVaultService = new KeyVaultService();
//        }

//        public override string GetValue(string key)
//        {
//            if (EnvironmentHelper.IsAzure && _allowedKeys.Contains(key))
//            {
//                return _keyVaultService.GetSecret(key);
//            }

//            return null;
//        }

//        public override ICollection<KeyValuePair<string, string>> GetAllValues(string prefix)
//        {
//            throw new NotImplementedException("Greedy mode is not supported");
//        }
//    }
//}