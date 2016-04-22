using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolusionFileCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderStatus = new List<string>
            {
                "Cancelled",
                "Partially Returned",
                "Partially Shipped",
                "Pending Shipment",
                "Returned",
                "See Order Notes",
                "Shipped"
            };

            var files = new List<Tuple<string, string, int>>
            {
                new Tuple<string, string, int>("BH", @"C:\temp\Volusion\{0}_v21886_orders_2016-04-11_530am.csv", 773323),
                new Tuple<string, string, int>("CTM", @"C:\temp\Volusion\{0}_v49650_orders_2016-04-11_140am.csv", 126500),
                new Tuple<string, string, int>("TC", @"C:\temp\Volusion\{0}_v774337_orders_2016-04-11_4am.csv", 166415)
            };

            var missingOrders = new Dictionary<string, ICollection<int>>
            {
                { "BH", new List<int> { 171147, 172533, 173836, 180675, 186516, 186968, 187262, 187374, 187617, 187670, 188637, 188693, 190408, 190789, 191579, 192998, 197012, 197023, 200755, 201507, 204120, 290807, 290808, 290809, 290810, 390250, 414106, 414107, 414108, 414109 } },
                { "CTM", new List<int>() },
                { "TC", new List<int> { 52158 } }
            };

            foreach (var file in files)
            {
                Console.WriteLine("Starting {0}...", file.Item1);

                var newFile = new List<string>();
                var orderIdAndStatuses = new List<string>();
                var wrappedLine = false;

                using (var sr = new StreamReader(String.Format(file.Item2, String.Empty)))
                {
                    do
                    {
                        var line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        if (wrappedLine)
                        {
                            newFile[newFile.Count - 1] = (newFile.Last() + line).Replace("\"", String.Empty);
                            wrappedLine = false;
                        }
                        else
                            newFile.Add(line.Replace(Environment.NewLine, String.Empty).Replace("\"", String.Empty));

                        if (!line.EndsWith(",NULL") && !line.EndsWith(",Y"))
                            wrappedLine = true;

                    } while (!sr.EndOfStream);
                }

                Console.WriteLine("Validating order IDs...");

                var firstOrderID = Convert.ToInt32(newFile.First().Substring(0, newFile.First().IndexOf(',')));
                var tempList = new List<int>();
                for (var idx = 1; idx < newFile.Count; idx++)
                {
                    var currentOrderID = Convert.ToInt32(newFile[idx].Substring(0, newFile[idx].IndexOf(',')));
                    if (firstOrderID + idx != currentOrderID)
                    {
                        var diff = (currentOrderID - (firstOrderID + idx));
                        for (var cnt = 0; cnt < diff; cnt++)
                        {
                            tempList.Add(firstOrderID + idx);
                            ++firstOrderID;
                        }
                    }
                }

                if ((missingOrders[file.Item1].Any() || tempList.Any()) && (missingOrders[file.Item1].Count != tempList.Count || !tempList.Any(x => missingOrders[file.Item1].Contains(x))))
                    throw new Exception("Found a missing order that is not known: " + String.Join(", ", tempList.Except(missingOrders[file.Item1])));

                Console.WriteLine("Validating CSV data...");

                var csvDataParser = new CsvDataParser();
                var missingStatuses = new List<string>();
                
                foreach (var line in newFile)
                {
                    var csvLineData = csvDataParser.Execute(line, ",");
                    var orderID = csvLineData.Fields.ElementAt(0).Value;

                    //int shippedIdx = 42, shipDateIdx = 46, status = 54;
                    string status = String.Empty, shipped = String.Empty, shipDate = String.Empty;

                    var searchedStatus = csvLineData.Fields.FirstOrDefault(x => orderStatus.Contains(x.Value));
                    if (searchedStatus == null)
                        missingStatuses.Add(orderID);
                    else
                    {
                        var statusFieldIdx = searchedStatus.Position.Value;

                        status = searchedStatus.Value;
                        shipped = csvLineData.Fields.ElementAt(statusFieldIdx - 12).Value;
                        shipDate = csvLineData.Fields.ElementAt(statusFieldIdx - 8).Value;
                    }

                    orderIdAndStatuses.Add(orderID + "," + shipped + "," + shipDate + "," + status);

                    if (orderID == file.Item3.ToString()) // reached the cutoff
                        break;
                }

                if (missingStatuses.Any())
                    Console.WriteLine("Missing order status: " + String.Join(", ", missingStatuses));

                Console.WriteLine("Writing file...");

                using (var sw = new StreamWriter(String.Format(file.Item2, file.Item1)))
                {
                    sw.WriteLine("OrderID,Shipped,ShipDate,OrderStatus");

                    foreach (var line in orderIdAndStatuses)
                        sw.WriteLine(line);

                    sw.Flush();
                }

                Console.WriteLine("Done");
            }
        }
    }
}
