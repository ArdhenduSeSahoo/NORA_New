using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class GroupTemplate : Template
    {
        public GroupDisplayType DisplayType { get; set; }
        public List<GroupTemplatePart> Parts { get; set; }
    }

    public enum GroupDisplayType
    {
        VerticalLineUp = 1,
        Grid = 2,
        Tabs = 3
    }

    public class GroupTemplatePart
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        [Translatable()]
        public string Title { get; set; }
        public ContainerType ContainerType { get; set; }
        public bool CollapsedByDefault { get; set; }
        public int GroupTemplateId { get; set; }
        public GroupTemplate GroupTemplate { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public GridPositions GridPositions { get; set; }
        public PopupWidthType PopupWidthType { get; set; }
        public bool AddNewRowAtTheEnd { get; set; }
        public int DefaultRowHeight { get; set; }
    }

    public enum PopupWidthType
    {
        Unspecified = 0,
        Small = 1,
        Large = 2,
        AsBigAsPossible = 3
    }

    public enum ContainerType
    {
        None = 0,
        CollapsiblePanel = 1,
        FixedPanel = 2,
        Popup = 3
    }
}
