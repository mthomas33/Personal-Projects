using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolusionFileCleaner
{
    public class FileContents : IFileContents
    {
        public FileContents(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
        public string Contents { get; set; }
        public IEnumerable<ILineData> Lines { get; set; }
    }
}
