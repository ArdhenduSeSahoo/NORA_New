using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    /// <summary>
    /// Represents signature to be followed while tracking an entity which is being created.
    /// </summary>
    public interface IAddTracking
    {
        /// <summary>
        /// DateTime recorded while creating an entity.
        /// </summary>
        DateTime AddDateTime { get; set; }

        /// <summary>
        /// User details recorded for an entity tracking entry while adding an entity. 
        /// </summary>
        UserManagement.User AddUser { get; set; }
        int? AddUserId { get; set; }
    }

    /// <summary>
    /// Represents signature to be followed while modifying the existing tracking entry of an entity.
    /// </summary>
    public interface IUpdateTracking
    {
        /// <summary>
        /// DateTime recorded while modifying an entity.
        /// </summary>
        DateTime EditDateTime { get; set; }

        /// <summary>
        /// User details recorded for an entity track entry while modifying an entity. 
        /// </summary>
        UserManagement.User EditUser { get; set; }
        int? EditUserId { get; set; }
    }

    /// <summary>
    /// Interface inherits IAddTracking and IUpdateTracking.
    /// </summary>
    public interface IModifyTracking : IAddTracking, IUpdateTracking
    {
    }


}
