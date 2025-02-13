using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class InboundServiceCall : IEntity
    {
        public int Id { get; set; }
        public string ErrorMessage { get; set; }
        public byte[] PayLoad { get; set; }
        public string PayLoadLink { get; set; }
        public string PayLoadLinkHost { get; set; }
        public string HeadersAsString { get; set; }
        public Dictionary<string, string[]> Headers
        {
            get { return this.HeadersAsString == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string[]>>(this.HeadersAsString); }
            set { this.HeadersAsString = value == null ? null : JsonConvert.SerializeObject(value); }
        }
        public DateTime StartReceivingDateTime { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public DateTime? ProcessedDateTime { get; set; }
        public string UserName { get; set; }
        [MaxLength(50)]
        public string Method { get; set; }
        public string Uri { get; set; }
        public string ContentType { get; set; }
        public byte[] ResultPayLoad { get; set; }
        public string ResultPayLoadLink { get; set; }
        public string ResultPayLoadLinkHost { get; set; }
        public int? ForeignClientId { get; set; }
        public ForeignClient ForeignClient { get; set; }

        [MaxLength(50)]
        public string UniqueId { get; set; } //Header: UniqueID
        [MaxLength(50)]
        public string CorrelationId { get; set; } //Header: CorrelationID
        [MaxLength(50)]
        public string MessageType { get; set; } //Header: MsgType

    }
}
