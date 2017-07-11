using System.Collections.Generic;
using System.Linq;

namespace SimpleSearch
{
    public sealed class SearchIndex<TType>
    {
        internal SearchIndexOptions<TType> Options { get; set; }
        internal List<SearchItem<TType>> Source { get; set; } = new List<SearchItem<TType>>();

        public IEnumerable<RankedResult<TType>> RankedSearch(string searchQuery)
        {
            var cleanedQuery = searchQuery.Split(' ').Select(x => x.Clean()).ToArray();
            var ranks = Source.Select(i => PerformSearch(i, cleanedQuery)).Where(r=> r != null);

            return ranks.OrderByDescending(c=> c.Score);
        }
        
        public IEnumerable<TType> Search(string searchQuery)
        {
            return RankedSearch(searchQuery).Select(r => r.Item);
        }

        /// <summary>
        /// Finds the best matching item
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public TType Find(string searchQuery)
        {
            var topResult = RankedSearch(searchQuery).FirstOrDefault();
            if (topResult == null)
                return default(TType);

            return topResult.Item;
        }

        private Dictionary<string, double> SearchProperties = new Dictionary<string, double>();
        private RankedResult<TType> PerformSearch(SearchItem<TType> item, string[] searchQuery)
        {
            SearchProperties.Clear();

            foreach (var indexedProperty in item.Properties)
            {
                var option = Options.Properties[indexedProperty.Key];

                foreach (var searchWord in searchQuery)
                {
                    double score = 0;

                    if (indexedProperty.Value.Contains(searchWord))
                    {
                        score = option.Score * (searchWord.Length) * (option.RequiresFullWordMatch ? 2 : 1);
                    }else if(!option.RequiresFullWordMatch && indexedProperty.Value.Any(v=> v.Contains(searchWord)))
                    {
                        score = (option.Score * (searchWord.Length)) / 2;
                    }

                    if(score > 0)
                    {
                        // Increment the score value
                        if (SearchProperties.ContainsKey(indexedProperty.Key))
                            SearchProperties[indexedProperty.Key] += score;
                        else
                            SearchProperties.Add(indexedProperty.Key, score);
                    }
                }
            }

            if (SearchProperties.Any())
            {
                return new RankedResult<TType>
                {
                    Item = item.Item,
                    Properties = SearchProperties.ToDictionary(p=> p.Key, p=> p.Value)
                };
            }
            return null;
        }
    }
}
