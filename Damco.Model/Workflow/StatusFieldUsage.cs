using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    public class StatusFieldUsage
    {
        public int Id { get; set; }
        public int DataFieldId { get; set; }
        public DataField DataField { get; set; }
        public FieldUsages Usage { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int ProfileId { get; set; }
        public StatusUsageProfile Profile { get; set; }
    }

    public class StatusDataSourceUsage
    {
        public int Id { get; set; }
        public int DataSourceId { get; set; }
        public DataSource DataSource { get; set; }
        public DataSourceUsages Usage { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int ProfileId { get; set; }
        public StatusUsageProfile Profile { get; set; }
    }

    [Flags()]
    public enum FieldUsages
    {
        Hidden = 1,
        Readonly = 2,
        Optional = 4,
        Mandatory = 8,
        RequiredForStatusChange = 16
    }

    [Flags()]
    public enum DataSourceUsages
    {
        //TODO: Add logic for the commented out stuff, right now we only support readonly or a combination of Add / Delete
        //Hidden = 0,
        Readonly = 1,
        //Edit = 2,s
        Add = 4,
        Delete = 8
    }

    public class StatusUsageProfile
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public List<StatusFieldUsage> Fields { get; set; } = new List<StatusFieldUsage>();
        public List<StatusDataSourceUsage> DataSources { get; set; } = new List<StatusDataSourceUsage>();
    }

}

