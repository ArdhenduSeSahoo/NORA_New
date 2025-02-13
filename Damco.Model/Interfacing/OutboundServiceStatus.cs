using System;
using System.ComponentModel.DataAnnotations;

namespace Damco.Model.Interfacing
{
    public class OutboundServiceCallStatus : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string ForeignApi { get; set; }

        [Required]
        public bool IsSent { get; set; }

        [Required]
        public DateTime? EnqueuedTime { get; set; }

        public DateTime? SentTime { get; set; }

        public string ErrorMessage { get; set; }

        public string ResultPayload { get; set; }

        public string FileName { get; set; }
        
        [Required]
        public int Count { get; set; }

        public int OutboundServiceCallId { get; set; }

        public OutboundServiceCall OutboundServiceCall { get; set; }
    }
}