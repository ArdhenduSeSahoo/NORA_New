using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class InterfaceSetup : IEntity, ILogged<InterfacingMasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required()]
        public string Name { get; set; }
        public List<InterfaceSetupAlias> Aliases { get; set; }
        public int? InboundMessageErrorStatusId { get; set; }
        public Status InboundMessageErrorStatus { get; set; }
        public int? UpdatePreparationOperationId { get; set; }
        public int? CreatePreparationOperationId { get; set; }
        public int? CurrentVersionPreparationOperationId { get; set; }
        public int? UpdateNewVersionPreparationOperationId { get; set; }
        public int? CreateNewVersionPreparationOperationId { get; set; }

        //public Operation PreparationOperation { get; set; }
        public int? CreatedOperationId { get; set; }
        //public Operation CreatedOperation { get; set; }
        public int? UpdatedOperationId { get; set; }
        //public Operation UpdatedOperation { get; set; }
        public bool IgnoreRepeatedContent { get; set; }

        public int? ParentId { get; set; }
        public InterfaceSetup Parent { get; set; }

        public int? MessageFlowSetupId { get; set; }
        public InterfaceSetup MessageFlowSetup { get; set; }

        public bool OnlyUpdateExisting { get; set; }
        public bool IgnoreDataUpdates { get; set; } //Operations only
        public bool ProcessOverlookRequiresAdministrator { get; set; }
        public bool AliasOverlookRequiresAdministrator { get; set; }
    }

    public class InterfaceSetupAlias : IEntity, ILogged<InterfacingMasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Alias { get; set; }
        public int InterfaceSetupId { get; set; }
        [CascadeDelete()]
        public InterfaceSetup InterfaceSetup { get; set; }
    }

}
