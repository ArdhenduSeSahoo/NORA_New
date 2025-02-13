using System.ComponentModel.DataAnnotations;
using Damco.Model;

namespace NORA.Model
{
    public class VoyageName : IEntity
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
    }
}
