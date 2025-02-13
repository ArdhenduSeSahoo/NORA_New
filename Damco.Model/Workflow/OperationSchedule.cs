using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    public class OperationSchedule : IEntity, ILogged<WorkflowSetupLog>
    {
        public int Id { get; set; }
        public int OperationId { get; set; }
        //public Operation Operation { get; set; }
        public int? SecondsBetweenRuns { get; set; }
        public DateTime? TimeOfDay { get; set; }
        public int? DayOfMonth { get; set; }
        public DaysOfWeek? DaysOfWeek { get; set; }
        [NotMapped()]
        public Type EntityType
        {
            get { return this.EntityTypeAsString == null ? null : ExpressionSerialization.GetTypeForName(this.EntityTypeAsString); }
            set { this.EntityTypeAsString = value == null ? null : ExpressionSerialization.GetNameForType(value); }
        }
        public string EntityTypeAsString { get; set; }
        public bool IsActive { get; set; }
    }

    public class OperationScheduleStatus : IEntity
    {
        [Key(), ForeignKey(nameof(OperationSchedule))]
        public int OperationScheduleId { get; set; }
        public OperationSchedule OperationSchedule { get; set; }
        public DateTime? LastRun { get; set; }
    }

    public enum OperationScheduleLogEvent
    {
        Started = 1,
        Finished = 2,
        BusinessError = 3,
        Error = 4
    }

    public class OperationScheduleLog : IEntity
    {
        public int Id { get; set; }
        public int OperationScheduleId { get; set; }
        public OperationScheduleLogEvent Event { get; set; }
        public DateTime DateTime { get; set; }
        [NotMapped()]
        public TextPlaceHolder Info
        {
            get { return this.InfoAsString.FromJson<TextPlaceHolder>(); }
            set { this.InfoAsString = value.ToJson(); }
        }
        public string InfoAsString { get; set; }
    }

    [Flags()]
    public enum DaysOfWeek
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64
    }
}
