using System;
namespace VolusionFileCleaner
{
    public interface IFileContents
    {
        string Contents { get; }
        string FileName { get; }
        System.Collections.Generic.IEnumerable<ILineData> Lines { get; }
    }
}
