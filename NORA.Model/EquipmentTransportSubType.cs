using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class EquipmentTransportSubType: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
    }

    public class EquipmentTransportSubTypeAlias: AliasBase<EquipmentTransportSubType>, IEntity
    {
    }
}
