using System;
namespace VolusionFileCleaner
{
    public interface IFieldData
    {
        int? Position { get; }
        string Value { get; }
    }
}
