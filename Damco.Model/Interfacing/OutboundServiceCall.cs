using Damco.Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class OutboundServiceCall : IEntity
    {
        public OutboundServiceCall()
        {
            OutboundServiceCallStatuses = new List<OutboundServiceCallStatus>();
        }

        public int Id { get; set; }
        public DateTime EnqueDateTime { get; set; }
        public DateTime? StartSendingDateTime { get; set; }
        public DateTime? SentDateTime { get; set; }
        public byte[] PayLoad { get; set; }
        public string PayLoadLink { get; set; }
        public string PayLoadLinkHost { get; set; }
        public ServiceCallMethod Method { get; set; }
        public string BaseUri { get; set; }
        public string ResourceUri { get; set; }
        public int ForeignApiId { get; set; }
        public ForeignApi ForeignApi { get; set; }
        public string HeadersAsString { get; set; }

        public int? BackgroundProcessId { get; set; }
        public int? UserId { get; set; }
        public string SequenceNumber { get; set; }

        [NotMapped()]
        public Dictionary<string, string[]> Headers
        {
            get { return this.HeadersAsString == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string[]>>(this.HeadersAsString); }
            set { this.HeadersAsString = value == null ? null : JsonConvert.SerializeObject(value); }
        }
        public string ErrorMessage { get; set; }
        public byte[] ResultPayLoad { get; set; }
        public string ResultPayLoadLink { get; set; }
        public string ResultPayLoadLinkHost { get; set; }
        public int? OperationId { get; set; }
        public string ContentType { get; set; }
        //public Operation Operation { get; set; } //We don't want the have the relationship as this is mostly a log table

        [MaxLength(50)]
        public string UniqueId { get; set; }
        [MaxLength(50)]
        public string CorrelationId { get; set; }
        [MaxLength(50)]
        public string MessageType { get; set; }
        public string PayloadHash { get; set; }
        public OutboundServiceCallType? OutboundServiceCallType { get; set; }

        public ICollection<OutboundServiceCallStatus> OutboundServiceCallStatuses { get; set; }
    }

}
