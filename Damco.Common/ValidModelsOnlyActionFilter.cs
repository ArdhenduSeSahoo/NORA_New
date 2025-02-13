//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;

//namespace Damco.Common
//{
//    public class ValidModelsOnlyActionFilter: ActionFilterAttribute
//        {
//            public override void OnActionExecuting(HttpActionContext actionContext)
//            {
//                if (actionContext.ModelState.IsValid)
//                {
//                    base.OnActionExecuting(actionContext);
//                }
//                else
//                {
//                    var exceptions = new List<Exception>();

//                    foreach (var state in actionContext.ModelState)
//                    {
//                        if (state.Value.Errors.Count != 0)
//                        {
//                            exceptions.AddRange(state.Value.Errors.Select(error => error.Exception).Where(e => e != null));
//                        }
//                    }

//                    if (exceptions.Count == 1)
//                        throw exceptions.First();
//                    else if (exceptions.Count > 0)
//                        throw new AggregateException(exceptions);
//                }
//            }


//    }
//}
