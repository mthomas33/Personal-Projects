using System;
using System.Collections.Generic;

namespace VolusionFileCleaner
{
    public class CsvDataParser : ICsvDataParser
    {
        public ILineData Execute(string csvData, string delimiter)
        {
            var parsedLineData = new List<FieldData>();
            var lineData = new LineData { Fields = parsedLineData };
            var positionCounter = 0;
            var currIdx = 0;

            // in case somebody sends an item with double-quotes back to back, it means the text needs to have a single double-quote; so replace it with a token
            var doubleQuoteToken = "{####}";
            var scrubbedCsvData = csvData
                .Replace("\"\"", doubleQuoteToken)
                .Replace("," + doubleQuoteToken + ",", ",\"\",")
                .Replace("," + doubleQuoteToken, ",\"\"")
                .Replace(doubleQuoteToken + ",", "\"\","); // ensure if somebody sends an empty string ex: "blahblah","","blah" 

            try
            {
                while (true)
                {
                    var fieldData = new FieldData { Position = positionCounter++ };
                    parsedLineData.Add(fieldData);

                    var startQuoteIdx = scrubbedCsvData.IndexOf("\"", currIdx);
                    var delimIdx = scrubbedCsvData.IndexOf(delimiter, currIdx);
                    if (delimIdx == -1)
                        delimIdx = scrubbedCsvData.Length;

                    if (startQuoteIdx > -1 && startQuoteIdx < delimIdx)
                    {
                        var endQuoteIdx = scrubbedCsvData.IndexOf("\"", startQuoteIdx + 1);
                        fieldData.Value = scrubbedCsvData.Substring(startQuoteIdx + 1, endQuoteIdx - startQuoteIdx - 1).Trim();

                        delimIdx = scrubbedCsvData.IndexOf(delimiter, endQuoteIdx);
                        if (delimIdx == -1)
                            delimIdx = scrubbedCsvData.Length;
                    }
                    else
                        fieldData.Value = scrubbedCsvData.Substring(currIdx, delimIdx - currIdx).Trim();

                    currIdx = delimIdx + 1;

                    // replace double-quote token with single double-quote
                    fieldData.Value = fieldData.Value.Replace(doubleQuoteToken, "\"");

                    if (currIdx >= scrubbedCsvData.Length)
                        break;
                }
            }
            catch (Exception ex)
            {

            }

            return lineData;
        }
    }
}
