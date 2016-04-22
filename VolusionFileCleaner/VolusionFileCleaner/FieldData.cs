using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolusionFileCleaner
{
    public class FieldData : IFieldData
    {
        public int? Position { get; set; }
        public string Value { get; set; }
    }
}
