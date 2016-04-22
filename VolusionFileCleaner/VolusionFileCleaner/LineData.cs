using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolusionFileCleaner
{
    public class LineData : ILineData
    {
        public int? LineNumber { get; set; }
        public IEnumerable<IFieldData> Fields { get; set; }
    }
}
