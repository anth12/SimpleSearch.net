using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleSearch.Tests.Models;
using System.Linq;

namespace SimpleSearch.Tests
{
    [TestClass]
    public class IndexerTests
    {
        private readonly SearchIndex<SampleClass> SearchIndex;

        public IndexerTests()
        {
            SearchIndex = SearchIndexer.Build(Mock.SimpleData,
                new SearchIndexOptions<SampleClass>(resultThreshold: 0.5d)
                    .AddProperty(c => c.Company, 1)
                    .AddProperty(c => c.City)
                    .AddProperty(c => c.Country, 0.6)
            );
        }

        [TestMethod]
        public void Can_find_US_companies()
        {
            var results = SearchIndex.Search("United States of America").ToList();

            Assert.AreEqual(5, results.Count());
        }
    }
}
