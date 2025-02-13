using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.FileBuilding
{
    public class RawDataFileDefinition : DataSourcedFileBuildingDefinition
    {
        public RawDataFileType Type { get; set; }
    }

    public enum RawDataFileType
    {
        Json = 1,
        JsonIndented = 2,
        DataSetXml = 3,
        JsonSample = 4,
        JsonSampleIndented = 5
    }
}
