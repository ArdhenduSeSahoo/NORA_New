using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    /// <summary>
    /// Specifies the result of a save operation.
    /// </summary>
    [Serializable]
    public enum SaveResult
    {
        NoChange = 0,
        Created = 1,
        Updated = 2,
        Deleted = 3,
        //UpdatePendingUserChoice = 4,
        //UpdateNotAllowed = 5,
        ErrorsFound = 6,
        PreviousChangeReversed = 7,
        PartsMissing = 8,
        CorrelatedItemsPending = 9,
        CorrelatedItemsStarted = 10,
        HandledButDataUpdatesIgnored = 11,
        Preignored = 12
    }
}
