using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Diagnostics;
using SimpleSearch.Tests.Models;

namespace SimpleSearch.Tests
{
    [TestClass]
    public class AirportSearchTests
    {
        public AirportSearchTests()
        {
            SearchIndex = SearchIndexer.Build(Mock.Airports,
                new SearchIndexOptions<Airport>()
                    .AddProperty(c => c.iata_code, 1, true)
                    .AddProperty(c => c.name)
                    .AddProperty(c => c.municipality, 0.6)
                    .AddProperty(c => c.iso_country, 0.4, true)
            );
        }
        
        public SearchIndex<Airport> SearchIndex;

        [TestMethod]
        public void Can_find_heathrow_by_code()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var results = SearchIndex.Search("LHR").ToList();

            stopWatch.Stop();

            Assert.AreEqual(results.Count, 1);
        }

        [TestMethod]
        public void Can_find_malta_by_name()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var results = SearchIndex.Search("Malta").ToList();

            stopWatch.Stop();

            Assert.AreEqual(results.Count, 2);

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            File.AppendAllLines(@"..\..\perf.txt", new[] { $"v{version}: {stopWatch.Elapsed}" });
        }

        [TestMethod]
        public void Can_find_london_airports()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var results = SearchIndex.Search("london airport").ToList();

            stopWatch.Stop();

            Assert.AreEqual(results.Count, 6);

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            File.AppendAllLines(@"..\..\perf.txt", new[] { $"v{version}: {stopWatch.Elapsed}" });
        }

        [TestMethod]
        public void Can_find_manchester_us()
        {
            var results = SearchIndex.Search("manchester, us").ToList();
            
            Assert.AreEqual(results.Count, 1);
        }

        [TestMethod]
        public void Can_find_manchester_gb()
        {
            var results = SearchIndex.Search("manchester, gb").ToList();

            Assert.AreEqual(results.Count, 1);
        }


        [TestMethod]
        public void Can_handle_no_results()
        {
            var results = SearchIndex.Search("extravagant crossword").ToList();

            Assert.AreEqual(results.Count, 1);
        }
    }
}
