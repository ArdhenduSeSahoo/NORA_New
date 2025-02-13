using Damco.Model.DataSourcing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.DataValidation
{
    public abstract class DataValidationField
    {
        public int Id { get; set; }

        //TODO: Replace this with actual link to DataFields table, after we have the templates in configuration i/o code
        //public int? DataFieldId { get; set; }
        //public DataField DataField { get; set; }
        public string DataFieldTag { get; set; }
        public string DataFieldSourceAsString { get; set; } //TODO: Replace this with actual link to DataFields table, after we have the templates in configuration i/o code
        public string FieldDisplayName { get; set; }
        [MaxLength(200), Required()]
        public string EntityTypeAsString { get; set; } //done via EntityType
        [NotMapped()]
        public Type EntityType 
        {
            get { return ExpressionSerialization.GetTypeForName(this.EntityTypeAsString); }
            set { this.EntityTypeAsString = ExpressionSerialization.GetNameForType(value); }
        }
        //if there is an _ in the alias: type.GetType("NORA.Model." + everythingbeforetheunderscore)
        //if there is no underscore: typeof(TransportationDocument)

        public int EntityId { get; set; }
        public string CorrectValueAsString { get; set; }
        [NotMapped()]
        public object CorrectValue
        {
            get { return this.CorrectValueAsString == null ? null : JsonConvert.DeserializeObject(this.CorrectValueAsString); }
            set { this.CorrectValueAsString = value == null ? null : JsonConvert.SerializeObject(value); }
        }
        public bool ValueIsCorrect { get; set; }

        public string CheckValueAsString { get; set; }
        [NotMapped()]
        public object CheckValue
        {
            get { return this.CheckValueAsString == null ? null : JsonConvert.DeserializeObject(this.CheckValueAsString); }
            set { this.CheckValueAsString = value == null ? null : JsonConvert.SerializeObject(value); }
        }

        public string EntityReference { get; set; }
        public string Comments { get; set; }
    }
}
