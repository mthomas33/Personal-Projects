using System;
namespace VolusionFileCleaner
{
    public interface ILineData
    {
        System.Collections.Generic.IEnumerable<IFieldData> Fields { get; }
        int? LineNumber { get; set; }
    }
}
