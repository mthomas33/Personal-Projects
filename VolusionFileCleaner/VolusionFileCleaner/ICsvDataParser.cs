namespace VolusionFileCleaner
{
    public interface ICsvDataParser
    {
        ILineData Execute(string csvData, string delimiter);
    }
}