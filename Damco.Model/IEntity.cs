using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    /// <summary>
    /// Interface just to mark a class as an entity.
    /// This is used to validate entities are used in certain situations, to find out
    /// which objects are entities and also to create extension methods for entities.
    /// </summary>
    public interface IEntity
    {
    }
}
