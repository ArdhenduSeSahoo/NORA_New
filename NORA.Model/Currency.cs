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
    public class Currency : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        public decimal DefaultExchangeRate { get; set; }
    }
    public class CurrencyAlias : AliasBase<Currency>, IEntity { }


    public class ExchangeRate : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public DateTime FromDateTime { get; set; } //Effective from this date until the next from date
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal Value { get; set; } //Multiplication from the currency to EUR
    }

}
