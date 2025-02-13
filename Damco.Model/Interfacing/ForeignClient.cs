using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class ForeignClient : IEntity, ILogged<InterfacingMasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [Required()]
        public string ClientId { get; set; }
        public List<ForeignClientInterfaceSetup> InterfaceSetups { get; set; }
    }

    public class ForeignClientInterfaceSetup: IEntity, ILogged<InterfacingMasterDataLog>
    {
        public int Id { get; set; }
        public int ForeignClientId { get; set; }
        [CascadeDelete()]
        public ForeignClient ForeignClient { get; set; }
        public int InterfaceSetupId { get; set; }
        [CascadeDelete()]
        public InterfaceSetup InterfaceSetup { get; set; }
    }


}
