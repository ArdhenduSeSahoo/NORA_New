using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class ForeignApi : IEntity, ILogged<InterfacingMasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// Root Url of the API - the part of the Url that changes if e.g. the target server is moved.
        /// </summary>
        public string Url { get; set; }

        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>() ?? new string[] { }; }
            set { this.TagsAsString = (value == null || value.Length == 0 ? null : value.ToJson()); }
        }
        public string TagsAsString { get; set; }
        public List<ForeignApiHeader> Headers { get; set; }

        public ApiAuthenticationMethod AuthenticationMethod { get; set; }

        public string TokenRequestUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SecurityResource { get; set; }

        public ForeignApiSpecialType SpecialType { get; set; }
        public string InstanceId { get; set; }

        public bool LogOnly { get; set; }
        public List<ForeignApiInstance> Instances { get; set; }
        public bool AllowManualTrigger { get; set; }

        [MaxLength(50)]
        public string ClientSecretKeyName { get; set; }
    }
    public enum ApiAuthenticationMethod
    {
        None = 0,
        Basic = 1,
        OAuth2ClientCredentials = 2
    }

    public enum ForeignApiSpecialType
    {
        None = 0,
        Frontend = 1,
        Backend = 2,
        EmailSender = 3,
        KeyVault = 4
    }

    public class ForeignApiHeader : IEntity, ILogged<InterfacingMasterDataLog>
    {
        public int Id { get; set; }
        public int ForeignApiId { get; set; }
        public ForeignApi ForeignApi { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        [Required()]
        public string Value { get; set; }
    }

    public class ForeignApiInstance: IEntity
    {
        public int Id { get; set; }
        [Required(), MaxLength(200), ForceUnique("Code_ForeignApiId")]
        public string Code { get; set; }
        //public string ABCD { get; set; }
        [ForceUnique("Code_ForeignApiId")]
        public int ForeignApiId { get; set; }
        public ForeignApi ForeignApi { get; set; }
    }


}
