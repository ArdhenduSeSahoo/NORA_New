using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public interface IInterfacedEntity : IEntity
    {
        int Id { get; set; }
        //bool HasPendingChangesFromInterface { get; set; }
        bool HasPendingErrorsFromInterface { get; set; }
    }

    public interface IPossiblyLimitedWorfklow : IEntity
    {
        bool LimitedWorkflow { get; set; }
    }

}
