using Damco.Model.ScreenTemplating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.UserManagement
{
    public class Role : IEntity, ILogged<UserManagementLog>
    {
        public int Id { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public List<RolePage> Pages { get; set; } = new List<RolePage>();
        public List<RoleOperation> Operations { get; set; } = new List<RoleOperation>();
    }

    public class RolePage : IEntity, ILogged<UserManagementLog>
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        [CascadeDelete()]
        public Role Role { get; set; }
        public string PageDesignTag { get; set; } //TODO replace with a "real" link to the page design later
        //public int PageDesignId { get; set; }
        //public PageDesign PageDesign { get; set; }
    }

    public class RoleOperation : IEntity, ILogged<UserManagementLog>
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        [CascadeDelete()]
        public Role Role { get; set; }
        public string PageDesignOperationTag { get; set; } //TODO replace with a "real" link to the page design operation later
        //public int PageDesignOperationId { get; set; }
        //public PageDesignOperation PageDesignOperation { get; set; }
    }
}
