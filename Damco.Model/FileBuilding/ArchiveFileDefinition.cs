using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.FileBuilding
{
    public class ArchiveFileDefinition : FileBuildingDefinition
    {
        public List<FileBuildingDefinition> ContentFiles = new List<FileBuildingDefinition>();
        public ArchiveType CompressionType { get; set; }
        public bool OnlyArchiveIfMultipleFiles { get; set; }
    }

    public enum ArchiveType
    {
        Zip = 1
    }

}
