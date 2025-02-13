using Damco.Model.Interfacing;
using Damco.Model.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.UserManagement
{
    [Serializable]
    public enum UserType
    {
        WindowsUser,
        ExternalUser
    }
    /// <summary>
    /// Maintains user related data.
    /// </summary>
    [Serializable()]
    public class User : IEntity, ILogged<UserManagementLog>
    {
        /// <summary>
        /// Gets or sets the unique Id of the User.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the User name.
        /// </summary>
        [Required(), StringLength(150)]
        public string UserName { get; set; }
        [Required()]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets type of the user.
        /// </summary>
        /// <remarks>
        /// UserType can be either WindowsUser or ExternalUser.
        /// </remarks>
        public UserType UserType { get; set; }

        /// <summary>
        /// Gets or sets Email address of the user.
        /// </summary>
        [Required(), StringLength(500)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets TimeZone ID of the User.
        /// </summary>
        [Required(), StringLength(50)]
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets Hash string of the user's Password.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets AuthenticationProvider which is required for Authenticating the user.
        /// </summary>
        public string AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets AuthenticationProviderKey associated to AuthenticationProvider which is required for Authenticating the user.
        /// </summary>
        public string AuthenticationProviderKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if it is a user or a guest.
        /// </summary>
        //TODO: More advanced security
        //    public bool IsUser { get; set; }//TODO: Remove once new role based logic works

        /// <summary>
        /// Gets or sets a value indicating if the logged in user is Admin.
        /// </summary>
        /// <remarks>
        /// If the logged in user is Admin, additional features will be available.
        /// </remarks>
        //   public bool IsAdmin { get; set; }//TODO: Remove once new role based logic works
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string CompanyWideCode { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public List<UserRole> Roles { get; set; } = new List<UserRole>();
    }

    public class UserAlias : AliasBase<User>, IEntity, ILogged<UserManagementLog> { }

    public class UserRole : IEntity, ILogged<UserManagementLog>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [CascadeDelete()]
        public User User { get; set; }
        public int RoleId { get; set; }
        [CascadeDelete()]
        public Role Role { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }

}
