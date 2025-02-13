//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Principal;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace Damco.Common
//{
//    public static class WebApiUtils
//    {
//        public static string GetUserName(this IIdentity identity)
//        {
//            return identity?.Name
//                ?? (identity as ClaimsIdentity)?.Claims.FirstOrDefault(x => x.Type == "appid")?.Value;
//        }
//    }
//}
