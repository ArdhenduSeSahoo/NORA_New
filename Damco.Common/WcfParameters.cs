using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Damco.Common
{
    internal class WcfParameters
    {
        public WcfParameters() { }
        public WcfParameters(string urlParameterString)
        {
            this.LoadFromString(urlParameterString);
        }
        internal class User
        {
            public string Name { get; set; }
            public string Password { get; set; }
        }
        public bool RESTful { get; set; }

        public int TimeOut { get; set; } = -1;
        public User[] Users { get; set; } = new User[] { };
        public string[] Clients { get; set; } = new string[] { };
        public void LoadFromString(string urlParameterString)
        {
            if (urlParameterString.StartsWith("[")) urlParameterString = urlParameterString.Substring(1);
            if (urlParameterString.EndsWith("]")) urlParameterString = urlParameterString.Substring(0, urlParameterString.Length - 1);
            foreach (var param in urlParameterString.Split(';').Select(p => Tuple.Create(p.SubstringBefore("=", StringFindOptions.FullTextIfNotFound), p.SubstringAfter("=", StringFindOptions.NullIfNotFound))))
            {
                if (param.Item1.ToLower() == "timeout")
                    this.TimeOut = int.Parse(param.Item2);
                else if (param.Item1.ToLower() == "user")
                    this.Users = this.Users.UnionSome(new User()
                    {
                        Name = HttpUtility.UrlDecode(param.Item2.SubstringBefore(":", StringFindOptions.FullTextIfNotFound)),
                        Password = HttpUtility.UrlDecode(param.Item2.SubstringAfter(":", StringFindOptions.NullIfNotFound))
                    }).ToArray();
                else if (param.Item1.ToLower() == "users")
                    this.Users = this.Users.Union(
                        param.Item2.Split(',')
                        .Select(u =>
                            new User()
                            {
                                Name = HttpUtility.UrlDecode(param.Item2.SubstringBefore(":", StringFindOptions.FullTextIfNotFound)),
                                Password = HttpUtility.UrlDecode(param.Item2.SubstringAfter(":", StringFindOptions.NullIfNotFound))
                            }
                        )).ToArray();
                else if (param.Item1.ToLower() == "client")
                    this.Clients = this.Clients.UnionSome(HttpUtility.UrlDecode(param.Item2)).ToArray();
                else if (param.Item1.ToLower() == "clients")
                    this.Clients = this.Clients.Union(param.Item2.Split(',').Select(c => HttpUtility.UrlDecode(c))).ToArray();
                else if (param.Item1.ToLower() == "restful")
                    this.RESTful = (param.Item2 == null || Convert.ToBoolean(param.Item2));
            }
        }

        static Regex _getUrlParameters = new Regex(@"\[[^[]{1,}\]$");

        internal static WcfParameters GetAndRemoveFromUrl(ref string url)
        {
            var match = _getUrlParameters.Match(url);
            if (match.Success)
            {
                url = _getUrlParameters.Replace(url, m => "");
                return new WcfParameters(match.Value);
            }
            else
                return new WcfParameters();
        }

        internal static WcfParameters Get(string url)
        {
            var match = _getUrlParameters.Match(url);
            if (match.Success)
                return new WcfParameters();
            else
                return new WcfParameters(match.Value);
        }

    }
}
