using Damco.Model.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class ScreenView : IEntity, IConditionallyLogged<ScreenTemplatingSetupLog>
    {
        public int Id { get; set; }
        bool IConditionallyLogged.ShouldBeLogged() { return this.UserId == null; } //We want to log shared views

        /// <summary>
        /// Null for shared views
        /// </summary>
        public int? UserId { get; set; }
        public User User { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        /// <summary>
        /// Format is determined by the client logic. Json is preferred.
        /// </summary>
        [Required()]
        public string ViewInfoAsString { get; set; }
        public int PageDesignId { get; set; }
        //Next TODO once we have the page designs in the DB
        //public PageDesign PageDesign { get; set; }
    }
}
