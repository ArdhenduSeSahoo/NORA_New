using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.DataSourcing
{

    public class DataSourcingMasterDataLog: ChangeLogBase, IEntity
    {
    }

    /// <summary>
    /// Maintains DataSource details.
    /// </summary>
    /// <remarks>
    /// Implements interface IEntity.
    /// </remarks>
    [Serializable()]
    public class StoreDataSource : IEntity, ILogged<DataSourcingMasterDataLog>
    {
        /// <summary>
        /// Gets or sets unique Datasource Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets query as CommandText.
        /// </summary>
        [Required()]
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets list of fields in/from datasource.
        /// </summary>
        /// <remarks>
        /// DataSourceId is used to maintain relation between DataSource and DataSourceFields.
        /// </remarks>
        public virtual List<StoreDataField> Fields { get; set; } = new List<StoreDataField>();

        /// <summary>
        /// Gets or sets Description of the CommandText result.
        /// </summary>
        public string Description { get; set; }
        public string MirroredEntityTypeAsString { get; set; }
        [NotMapped()]
        public Type MirroredEntityType
        {
            get { return ExpressionSerialization.GetTypeForName(this.MirroredEntityTypeAsString); }
            set { this.MirroredEntityTypeAsString = ExpressionSerialization.GetNameForType(value); }
        }

        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>() ?? new string[] { }; }
            set { this.TagsAsString = (value == null || value.Length == 0 ? null : value.ToJson()); }
        }
        public string TagsAsString { get; set; }

    }

    /// <summary>
    /// Maintains data related to DataSource Fields of a DataSource.
    /// </summary>
    /// <remarks>
    /// Implements interface IEntity.
    /// </remarks>

    [Serializable()]
    public class StoreDataField : IEntity, ILogged<DataSourcingMasterDataLog>
    {
        /// <summary>
        /// Gets or sets unique Id of a field.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets DataSourceId of a field.
        /// </summary>
        /// <remarks>
        /// This field is from the DataSource Entity.
        /// </remarks>
        public int StoreDataSourceId { get; set; }
        public StoreDataSource StoreDataSource { get; set; }

        /// <summary>
        /// Gets or sets DataType of a field as a string.
        /// </summary>
        [Required(), StringLength(200)]
        public string DataTypeAsString { get; set; }

        /// <summary>
        /// Gets Type of the dataTypeString using the ExpressionSerialization class.
        /// sets Type for the dataTypeString.
        /// </summary>
        /// <remarks>
        /// This field is excluded in the database mapping.
        /// </remarks>
        [NotMapped()]
        public Type DataType
        {
            get { return ExpressionSerialization.GetTypeForName(this.DataTypeAsString); }
            set { this.DataTypeAsString = ExpressionSerialization.GetNameForType(value); }
        }

        /// <summary>
        /// Gets or sets name of the field as mentioned in the data source.
        /// </summary>
        [Required(), StringLength(50)]
        public string NameInDataSource { get; set; }

        /// <summary>
        /// Gets or sets indicating if the datasource field is a key.
        /// </summary>
        public bool PartOfKey { get; set; }

    }
    public class CustomSql : IEntity
    {

    }

    }
