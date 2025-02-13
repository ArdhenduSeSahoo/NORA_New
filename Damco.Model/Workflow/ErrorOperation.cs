using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    [NotMapped()]
    public class ErrorOperation : Operation
    {
        public string ErrorMessage { get; set; }
        public bool CanBeOverlooked { get; set; }
        public bool OverlookRequiresAdministrator { get; set; }
        public int? RelevantFieldId { get; set; }
        public List<ErrorOperationRelevantField> RelevantFields { get; set; } = new List<ErrorOperationRelevantField>();
    }

    [NotMapped()]
    public class ErrorOperationRelevantField
    {
        public int Index { get; set; }
        public DataField DataField { get; set; }
    }
}
