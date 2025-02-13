using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class MenuOption
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public MenuOption Parent { get; set; }
        public int? OrderNumber { get; set; }
        [Translatable()]
        public string Title { get; set; }
        public List<MenuOption> Children { get; set; }

    }
}
