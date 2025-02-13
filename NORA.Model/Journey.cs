using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NORA.Model
{
    [Serializable()]
    public class JourneyLog : ChangeLogBase, IEntity { }

    [Serializable()]
    public class Journey : ILogged<JourneyLog>
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string VoyageReference { get; set; }

        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public bool Updated { get; set; }
    }

    [Serializable()]
    public class SubJourney : ILogged<JourneyLog>

    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public virtual Journey Journey { get; set; }
        public int CompanyAliasId { get; set; }
        public virtual CompanyAlias CompanyAlias { get; set; }
        public int TypeId { get; set; }
        public DateTime? OriginalPlannedStartDateTime { get; set; }
        public DateTime? OriginalPlannedEndDateTime { get; set; }
        public DateTime? LastPlannedStartDateTime { get; set; }
        public DateTime? LastPlannedEndDateTime { get; set; }
        public DateTime? ActualStartDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public bool Updated { get; set; }

        public virtual List<TransportLeg> Legs { get; set; } = new List<TransportLeg>();
    }
}
