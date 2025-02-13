using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class PollingKey: IEntity
    {
        public int Id { get; set; }
        [ForceUnique(), Required(), MaxLength(50)]
        public string Code { get; set; } //e.g. "MDMCompanies"
        public string LastValue { get; set; } //e.g. "2018-01-25H11:45:00.55644"
    }
}
