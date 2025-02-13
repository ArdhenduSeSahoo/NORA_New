using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.BackgroundProcessing
{
    public class UserMessage: IEntity 
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [CascadeDelete()]
        public UserManagement.User User { get; set; }
        public DateTime DateTime { get; set; }
        [Required()]
        public string Message { get; set; }
        public LogType MessageType { get; set; }
    }
}
