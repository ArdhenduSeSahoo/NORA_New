using Damco.Model.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.FileBuilding
{
    public class TemporaryFile : IEntity
    {
        public int Id { get; set; }
        [Required()]
        public string FileName { get; set; }
        [Required()]
        public byte[] FileData { get; set; }
        public DateTime CreationDateTime { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
