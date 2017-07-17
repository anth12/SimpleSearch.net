using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SimpleSearch.Tests.Models;

namespace SimpleSearch.Tests
{
    [TestClass]
    public class AirportSearchTests
    {
        public AirportSearchTests()
        {
            SearchIndex = SearchIndexer.Build(Mock.Airports,
                new SearchIndexOptions<Airport>(resultThreshold: 0.5)
                    .AddProperty(c => c.iata_code, 1, true)
                    .AddProperty(c => c.name)
                    .AddProperty(c => c.municipality, 0.6)
                    .AddProperty(c => c.iso_country, 0.2, true)
            );
        }
        
        public SearchIndex<Airport> SearchIndex;

        [TestMethod]
        public void Can_find_heathrow_by_code()
        {
            var results = SearchIndex.Search("LHR").ToList();
            
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void Can_find_malta_by_name()
        {
            var results = SearchIndex.Search("Malta").ToList();

            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void Can_find_london_airports()
        {
            var results = SearchIndex.Search("london").ToList();
            
            Assert.AreEqual(8, results.Count);
        }

        [TestMethod]
        public void Can_find_manchester_us()
        {
            var results = SearchIndex.Search("manchester, us").ToList();
            
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("US", results[0].iso_country);
            Assert.AreEqual("GB", results[1].iso_country);
        }

        [TestMethod]
        public void Can_find_manchester_gb()
        {
            var results = SearchIndex.Search("manchester, gb").ToList();

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("GB", results[0].iso_country);
            Assert.AreEqual("US", results[1].iso_country);
        }


        [TestMethod]
        public void Can_handle_no_results()
        {
            var results = SearchIndex.Search("extravagant crossword").ToList();

            Assert.AreEqual(0, results.Count);
        }
    }
}
