using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace NORA.Model
{
    public class CalendarType: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }

        [ForeignKey("CalendarTypeId")]
        public List<CalendarTypeDateRange> DateRanges { get; set; } = new List<CalendarTypeDateRange>();
    }

    public class CalendarTypeDateRange: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int CalendarTypeId { get; set; }
        public CalendarType CalendarType { get; set; }
        public int? OriginZoneId { get; set; }
        public Zone DestinationZone { get; set; }
        public int? DestinationZoneId { get; set; }
        public Zone OriginZone { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public string Notes { get; set; }

    }
}
