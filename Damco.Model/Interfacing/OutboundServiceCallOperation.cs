using Damco.Model.DataSourcing;
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
    [NotMapped()]
    public class OutboundServiceCallOperation : Operation, IRequiresDataExtract
    {
        public int DataExtractId { get; set; }
        public DataExtract DataExtract { get; set; }
        public int ForeignApiId { get; set; }
        public ForeignApi ForeignApi { get; set; }
        /// <summary>
        /// Url to the specific resource/action. It is added after the Url of the ForeignApi.
        /// </summary>
        public string Url { get; set; }
        public ServiceCallMethod Method { get; set; }
        public List<OutboundServiceCallOperationGroupingField> GroupingFields { get; set; } = new List<OutboundServiceCallOperationGroupingField>();
        public bool GroupPerItem { get; set; }
        public List<OutboundServiceCallOperationHeader> Headers { get; set; } = new List<OutboundServiceCallOperationHeader>();
        public List<OutboundServiceCallOperationUrlParameter> UrlParameters { get; set; } = new List<OutboundServiceCallOperationUrlParameter>();
        public List<OutboundServiceCallOperationValueChange> ValueChanges { get; set; } = new List<OutboundServiceCallOperationValueChange>();
        public string UniqueIdValue { get; set; }
        public DataExtractField UniqueIdField { get; set; }
        public string CorrelationIdValue { get; set; }
        public DataExtractField CorrelationIdField { get; set; }
        public string MessageTypeValue { get; set; }
        public DataExtractField MessageTypeField { get; set; }
        public bool GetFile { get; set; }
        public string ResultFileName { get; set; }
        public bool DontLog { get; set; }
        public int? PollingKeyId { get; set; }
        public PollingKey PollingKey { get; set; }
    }

    [NotMapped()]
    public class OutboundServiceCallOperationHeader
    {
        public int Id { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public int OutboundServiceCallId { get; set; }
        public OutboundServiceCallOperation OutboundServiceCallOperation { get; set; }
        public string Value { get; set; }
        public int? DataExtractFieldId { get; set; }
        public DataExtractField DataExtractField { get; set; }
    }

    [NotMapped()]
    public class ValuesToChange
    {
        public string[] DataPathToId { get; set; }

        public string[] DataPathToValue { get; set; }

        //Type of the DB entity that you want to have changed, for example "TransportLeg"
        public Type DatabaseEntityType { get; set; }

        //Just write what you want to change it to and it will set the type accordingly
        public string ChangeTo { get; set; } 
    }

    [NotMapped()]
    public class OutboundServiceCallOperationValueChange
    {
        public DataExtractField DataExtractPrimaryKeyField { get; set; }
        public DataExtractField DataExtractFieldToChange { get; set; }

        //Type of the DB entity that you want to have changed, for example "TransportLeg"
        public Type DatabaseEntityType { get; set; }
        //Type for the value you want to change
        public Type ChangedValueType { get; set; }
        //For example "False"/"True"/"3123"
        public string ChangeTo { get; set; }
    }

    [NotMapped()]
    public class OutboundServiceCallOperationUrlParameter
    {
        public int Id { get; set; }
        public int OutboundServiceCallId { get; set; }
        public OutboundServiceCallOperation OutboundServiceCallOperation { get; set; }
        public int DataExtractFieldId { get; set; }
        public DataExtractField DataExtractField { get; set; }
        public int Index { get; set; }
    }

    public class OutboundServiceCallOperationGroupingField
    {
        public int Id { get; set; }
        public int OutboundServiceCallId { get; set; }
        public OutboundServiceCallOperation OutboundServiceCallOperation { get; set; }
        public int DataExtractFieldId { get; set; }
        public DataExtractField DataExtractField { get; set; }
    }

    public enum ServiceCallMethod
    {
        RESTPost = 1,
        RESTPut = 2,
        RESTDelete = 3,
        RESTGet = 4
    }
}
