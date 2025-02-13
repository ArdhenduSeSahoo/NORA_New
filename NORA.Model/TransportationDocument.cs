using Damco.Model;
using Damco.Model.Communication;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class DocumentLog : ChangeLogBase, IEntity { }

    public class TransportationDocument : IEntity, ILogged<DocumentLog>, IInterfacedEntity
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public DocumentType Type { get; set; }
        [MaxLength(50)]
        public string Reference { get; set; }
        [MaxLength(50)]
        public string UniqueId { get; set; }
        public string DocumentStorageId { get; set; }
        public List<ShipmentGroupDocument> ShipmentGroups { get; set; } = new List<ShipmentGroupDocument>();
        public List<TransportDocument> Transports { get; set; } = new List<TransportDocument>();
        public List<DocumentCollectionOrderDetail> DocumentCollectionOrderDetails { get; set; } = new List<DocumentCollectionOrderDetail>();
        public List<DocumentValidationOrder> DocumentValidationOrders { get; set; } = new List<DocumentValidationOrder>();
        public List<DocumentCreationOrder> DocumentCreationOrders { get; set; } = new List<DocumentCreationOrder>();
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public bool? Negotiable { get; set; }
        public string ShipperRemarks { get; set; }
        public string AdditionalShipperRemarks { get; set; }
        public string DocumentShipperRemarks { get; set; }
        public bool ShipperRemarksHandled { get; set; }
        [MaxLength(50)]
        public string Version { get; set; }
        public bool? IsFinalVersion { get; set; }
        public List<TransportationDocumentAttribute> Attributes { get; set; } = new List<TransportationDocumentAttribute>();
        public List<TransportationDocumentDate> Dates { get; set; } = new List<TransportationDocumentDate>();
        public List<TransportationDocumentMeasurement> Measurements { get; set; } = new List<TransportationDocumentMeasurement>();
        public List<TransportationDocumentParty> Parties { get; set; } = new List<TransportationDocumentParty>();
        public bool DocumentIsReceived { get; set; }
        public DateTime? DraftReceivedDateTime { get; set; }
        public DateTime? FinalReceivedDateTime { get; set; }
        public List<WorkOrderDocument> WorkOrders { get; set; } = new List<WorkOrderDocument>();
        public bool ToOrder { get; set; }
        public List<ShipmentSplitDocument> ShipmentSplits { get; set; } = new List<ShipmentSplitDocument>();
        public DateTime? OriginalPrintDateTime { get; set; }
        public bool Cancelled { get; set; }
        public int? VersionNumber { get; set; }
        public string HandlerRemarks { get; set; }
        public DateTime? IssueDateTime { get; set; }
        [MaxLength(100)]
        public string PlaceOfIssue { get; set; }
        public List<TransportationDocumentChargeInfo> ChargeInfos { get; set; } = new List<TransportationDocumentChargeInfo>();
        public bool TelexRelease { get; set; }
        public DateTime? TelexReleaseDateTime { get; set; }
        [MaxLength(50)]
        public string ExportLicenseNumber { get; set; }
        public bool NVOCC { get; set; }
        /// <summary>
        /// Workaround for Cube Header Volume
        /// </summary>
        public decimal? CubeHeaderVolume { get; set; }
    }

    public class TransportationDocumentChargeInfo: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public string FreightsAndCharges { get; set; }
        public decimal? PrepaidAmount { get; set; }
        public decimal? CollectAmount { get; set; }
        public int TransportationDocumentId { get; set; }
        public TransportationDocument TransportationDocument { get; set; }
    }

    public class TransportationDocumentPartyContact : PartyContact<TransportationDocumentParty>, ILogged<DocumentLog>, IEntity { }
    public class TransportationDocumentParty : Party<TransportationDocumentPartyContact>, ILogged<DocumentLog>, IEntity, IPartyWithSpecialPartyType, IPartyWithDocumentAddressBlock
    {
        private string documentAddressBlockHeader;

        public int TransportationDocumentId { get; set; }
        [CascadeDelete()]
        public TransportationDocument TransportationDocument { get; set; }
        public List<TransportationDocumentPartyRequirement> Requirements { get; set; } = new List<TransportationDocumentPartyRequirement>();
        public string DocumentAddressBlockHeader 
        {
            get
            {
                if (documentAddressBlockHeader == null)
                    return null;
                else if (documentAddressBlockHeader.Contains("\n"))
                    return documentAddressBlockHeader;
                else
                    return String.Concat(documentAddressBlockHeader, "\n");
            }
            set
            {
                documentAddressBlockHeader = value;
            }
        }

        public string DocumentAddressBlockBody { get; set; }
        public SpecialPartyType? SpecialPartyType { get; set; }
    }

    public interface IPartyWithDocumentAddressBlock
    {
        string DocumentAddressBlockHeader { get; set; }
        string DocumentAddressBlockBody { get; set; }
    }

    public interface IPartyWithSpecialPartyType
    {
        int Id { get; set; }
        int? TypeId { get; set; }
        SpecialPartyType? SpecialPartyType { get; set; }
    }

    [Flags()]
    public enum SpecialPartyType
    {
        Shipper = 1,
        Consignee = 2,
        Notify1 = 4,
        Notify2 = 8,
        Notify3 = 16,
        Payer = 32,
        OriginalShipper = 64
    }

    public class TransportationDocumentAttribute : AttributeBase<TransportationDocument>, ILogged<DocumentLog>, IEntity { }
    public class TransportationDocumentDate : DateBase<TransportationDocument>, ILogged<DocumentLog>, IEntity { }
    public class TransportationDocumentMeasurement : MeasurementBase<TransportationDocument>, ILogged<DocumentLog>, IEntity { }

    public class TransportationDocumentPartyRequirement : IEntity, ILogged<DocumentLog>
    {
        public int Id { get; set; }
        public int TransportationDocumentPartyId { get; set; }
        [CascadeDelete()]
        public TransportationDocumentParty TransportationDocumentParty { get; set; }
        public int? LanguageId { get; set; }
        public Language Language { get; set; }
        public int? CommunicationChannelId { get; set; }
        public CommunicationChannel CommunicationChannel { get; set; }
        public int? CopiesRequired { get; set; }
        public int? OriginalsRequired { get; set; }
    }

    public class DocumentType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public List<DocumentTypeAlias> Aliases { get; set; } = new List<DocumentTypeAlias>();
        public bool Mandatory { get; set; }
        public int? SwitchDocumentTypeId { get; set; }
        public DocumentType SwitchDocumentType { get; set; }
        public bool UseOriginalShipperAsShipper { get; set; }
        public bool UseUltimateConsigneeAsConsignee { get; set; }
        public int? ModalityId { get; set; }
        public Modality Modality { get; set; }
        public DocumentStandardType StandardType { get; set; }
    }
    [Flags()]
    public enum DocumentStandardType
    {
        Unspecified = 0,
        House = 1,
        Master = 2
    }

    public class DocumentTypeAlias : AliasBase<DocumentType>, IEntity, ILogged<MasterDataLog> { }

    public class ShipmentGroupDocument : IEntity, ILogged<ShipmentGroupLog>
    {
        public int Id { get; set; }
        public int ShipmentGroupId { get; set; }
        [CascadeDelete()]
        public ShipmentGroup ShipmentGroup { get; set; }
        public int TransportationDocumentId { get; set; }
        [CascadeDelete()]
        public TransportationDocument TransportationDocument { get; set; }
    }

    public class TransportDocument : IEntity, ILogged<TransportLog>
    {
        public int Id { get; set; }
        public int TransportId { get; set; }
        [CascadeDelete()]
        public Transport Transport { get; set; }
        public int TransportationDocumentId { get; set; }
        [CascadeDelete()]
        public TransportationDocument TransportationDocument { get; set; }
    }

    public class ShipmentSplitDocument : IEntity, ILogged<ShipmentLog>
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        [CascadeDelete()]
        public TransportationDocument Document { get; set; }
        public int ShipmentSplitId { get; set; }
        [CascadeDelete()]
        public ShipmentSplit ShipmentSplit { get; set; }
    }

}
