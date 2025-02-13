using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Damco.Model.MultiTenancy;

namespace Damco.Model.Interfacing
{
    public class FeatureConfiguration : IEntity
    {
        public int Id { get; set; }

        [Required(), MaxLength(100)]
        public string Code { get; set; }

        [Required()]
        public Tenant Tenant { get; set; }

        [Required(), DefaultValue(false)]
        public bool Enable { get; set; }

        [MaxLength(150)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}