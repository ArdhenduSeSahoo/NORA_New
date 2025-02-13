using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Damco.Model
{
    /// <summary>
    /// A customized attribute that every entity must have.
    /// Used by code generation and also to provide the set (plural) name of the entity.
    /// </summary>
    /// <remarks>
    /// Inherits Attribute class.
    /// </remarks>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=false)]
    public class EntityAttribute: Attribute 
    {
        /// <summary>
        /// Gets or sets name of an entity.
        /// </summary>
        public string SetName { get; set; }

        /// <summary>
        /// Gets plural name of an entity of type T.
        /// </summary>
        /// <typeparam name="T">Represents Type declarations like Class, interface etc. </typeparam>
        /// <returns>
        /// Null - if there is no attribute.
        /// SetName - if attribute already exists.
        /// Plural name of the entity - Otherwise.
        /// </returns>
        public static string GetSetName<T>()
        {
            return GetSetName(typeof(T));
        }

        /// <summary>
        /// Checks if the Type (like Class, interface etc.) passed has any attributes.
        /// </summary>
        /// <param name="type">Represents Type declarations like Class, interface etc.</param>
        /// <returns>Returns bool if there is any custom attribute applied to the Type.</returns>
        public static bool IsEntity(Type type)
        {
            return type.GetCustomAttributes<EntityAttribute>(false).Any();
        }

        /// <summary>
        /// Sets plural name for an entity.
        /// </summary>
        /// <param name="type">Represents Type declarations like Class, interface etc.</param>
        /// <returns>
        /// Null - if there is no attribute.
        /// SetName - if attribute already exists.
        /// Plural name of the entity - Otherwise.
        /// </returns>
        public static string GetSetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(EntityAttribute), false);
            if (attr.Length == 0)
                return null;
            else if (((EntityAttribute)attr[0]).SetName != null)
                return ((EntityAttribute)attr[0]).SetName;
            else
                return TextProviderBase.MakePlural(type.Name);
        }

        /// <summary>
        /// Gets or sets DisplayName of an entity.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets ShortName of an entity.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets Description of an entity.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets SetDisplayName of an entity.
        /// </summary>
        public string SetDisplayName { get; set; }
    }
}
