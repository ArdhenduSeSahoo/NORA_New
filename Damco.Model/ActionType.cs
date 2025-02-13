using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    /// <summary>
    /// Actions performed from UI.
    /// </summary>
    [Serializable()]
    public enum ActionType
    {
        Close,
        Plus,
        Minus,
        Up,
        Down,
        Right,
        Left,
        First,
        Previous,
        End,
        Next,
        Move,
        Search,
        Edit,
        Folder,
        Utility,
        Recent,
        Recycle,
        Report,
        Trash,
        Document,
        New,
        Ok,
        Printer,
        Help,
        Info,
        Lock,
        Universe,
        Export,
        Zip,
        Send,
        Personate,
        Alert,
        Delete
    }
}
