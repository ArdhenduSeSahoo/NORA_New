using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class InboundMessageError : IEntity, IWorkflowControlled, ILogged<InboundMessageLog>
    {
        public int Id { get; set; }
        public int InboundMessageId { get; set; }
        [CascadeDelete()]
        public InboundMessage InboundMessage { get; set; }
        public int? DelegatedToId { get; set; }
        public InterfacingParty DelegatedTo { get; set; }
        public bool CanBeOverlooked { get; set; }
        public bool OverlookRequiresAdministrator { get; set; }
        public bool Overlook { get; set; }
        public bool IsSolved { get; set; }
        public string Description { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public int? VersionNumber { get; set; }
        public string Comments { get; set; }
        public bool SolvedManually { get; set; }
    }
}
