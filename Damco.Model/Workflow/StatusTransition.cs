using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    [NotMapped()]
    public class StatusTransition : Operation
    {
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public bool ChangeToPrevious { get; set; }
        public List<StatusTransitionFromStatus> FromStatuses { get; set; }
        public int? StatusUsageProfileId { get; set; }
        public StatusUsageProfile StatusUsageProfile { get; set; }
        public bool AllowChangeFromBlank { get; set; }
        public bool AllowAnyFromStatus { get; set; }
        public bool SkipErrorMesssageOnIncorectFromStatus { get; set; }
        public bool UpdateVersion { get; set; }
    }

    public class StatusTransitionFromStatus
    {
        public int Id { get; set; }
        public int StatusTransitionId { get; set; }
        public StatusTransition StatusTransition { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
    }

}
