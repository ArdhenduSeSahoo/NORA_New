using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Damco.Model.DataSourcing
{
    public class RunInfo
    {
        public Damco.Model.UserManagement.User CurrentUser { get; set; }
        public static RunInfo Current
        {
            get
            {
                throw new InvalidOperationException("This method is only to be used within queries that are executed by DataSourceService");
            }
        }
    }
}