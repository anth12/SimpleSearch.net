using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsvHelper;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SimpleSearch.Tests
{
    [TestClass]
    public class AirportSearchTests
    {
        public AirportSearchTests()
        {
            using (var file = File.OpenRead(@"..\..\airports.csv"))
            using (var textReader = new StreamReader(file))
            {
                var csv = new CsvReader(textReader);
                Airports = csv.GetRecords<Airport>().ToList();
            }

            SearchIndex = SearchIndexer.Build(Airports,
                new SearchIndexOptions<Airport>()
                    .AddProperty(c => c.iata_code, 1, true)
                    .AddProperty(c => c.name)
                    .AddProperty(c => c.municipality, 0.6)
                    .AddProperty(c => c.iso_country, 0.4)
            );
        }

        public List<Airport> Airports;
        public SearchIndex<Airport> SearchIndex;

        [TestMethod]
        public void Can_find_malta()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var results = SearchIndex.Search("Malta").ToList();

            stopWatch.Stop();
        }
    }
}
