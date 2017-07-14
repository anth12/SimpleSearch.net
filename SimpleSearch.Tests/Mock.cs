using CsvHelper;
using SimpleSearch.Tests.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleSearch.Tests
{
    internal class Mock
    {
        private static List<Airport> _airports;
        internal static List<Airport> Airports
        {
            get
            {
                if (_airports != null)
                    return _airports;

                using (var file = File.OpenRead(@"..\..\airports.csv"))
                using (var textReader = new StreamReader(file))
                {
                    var csv = new CsvReader(textReader);
                    _airports = csv.GetRecords<Airport>().Where(a => !string.IsNullOrEmpty(a.iata_code) && a.iata_code.Length > 2).ToList();
                }
                return _airports;
            }
        }
        
        internal static List<SampleClass> SimpleData = new List<SampleClass>
        {
            new SampleClass("Wallmart", "Bentonville, Arkansas", "United States"),
            new SampleClass("State Grid", "Beijing", "China"),
            new SampleClass("China National Petroleum", "Beijing", "China"),
            new SampleClass("Sinopec Group", "Beijing", "China"),
            new SampleClass("Royal Dutch Shell", "The Hague", "The Netherlands"),
            new SampleClass("Exxon Mobil", "Irving, Texas", "United States"),
            new SampleClass("Volkswagen", "Wolfsburg", "Germany"),
            new SampleClass("Toyota", "Toyota, Aichi", "Japan"),
            new SampleClass("Apple", "Cupertino, California", "United States"),
            new SampleClass("BP", "London", "United Kingdom"),
            new SampleClass("Berkshire Hathaway", "Omaha, Nebraska", "United States"),
            new SampleClass("McKesson", "San Francisco, California", "United States"),
            new SampleClass("Samsung Electronics", "Suwon", "South Korea")
        };

    }
}
