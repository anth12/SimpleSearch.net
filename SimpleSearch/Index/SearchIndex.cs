using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleSearch
{
    public sealed class SearchIndex<TType>
    {
        internal string[] CommonWords;
        internal SearchIndexOptions<TType> Options { get; set; }
        internal List<SearchItem<TType>> Source { get; set; } = new List<SearchItem<TType>>();

        public IEnumerable<KeyValuePair<TType, double>> RankedSearch(string searchQuery)
        {
            // "london|airport"
            var searchWords = string.Join("|", 
                                    searchQuery.Split(' ').Select(x => x.Clean()).ToArray()
                                );

            // (?<=^.*:.*)\|(london|airport)\|
            var full = new Regex($"(?<={Options.RegexSafeIndexSeperatorChar}){searchWords}(?={Options.RegexSafeIndexSeperatorChar})");

            // (?<=^.*:.*)(london|airport)
            var partial = new Regex($"{searchWords}");
            

            var rankings = from indexedItem in Source
                        let result = PerformSearch(indexedItem, full, partial)
                        where result.Item1 > 0
                        orderby result.Item1 descending
                        select new Tuple<TType, double, bool>(indexedItem.Item, result.Item1, result.Item2);
            
            // Filter matches of only common words when better matches are found
            if (!rankings.All(r => r.Item3))
                rankings = rankings.Where(r => !r.Item3);

            if (!rankings.Any())
                return new List<KeyValuePair<TType, double>>();

            
            var resultThreshold = rankings.Max(c => c.Item2) * Options.ResultThreshold;
            
            return rankings.Where(c=> c.Item2 > resultThreshold).Select(r=> new KeyValuePair<TType, double>(r.Item1, r.Item2));
        }
        

        public IEnumerable<TType> Search(string searchQuery)
        {
            return RankedSearch(searchQuery).Select(r => r.Key);
        }

        /// <summary>
        /// Finds the best matching item
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public TType Find(string searchQuery)
        {
            var topResult = RankedSearch(searchQuery).FirstOrDefault();
            if (topResult.Key == null)
                return default(TType);

            return topResult.Key;
        }

        private Dictionary<string, double> SearchProperties = new Dictionary<string, double>();
        private Tuple<double, bool> PerformSearch(SearchItem<TType> item, Regex fullMatchRegex, Regex partialMatchRegex)
        {
            SearchProperties.Clear();

            var score = 0d;
            var commonWordsOnly = true;

            var fullWordMatches = fullMatchRegex.Matches(item.Index);
            
            // TODO compare splitting on `|` and doing .Contans
            // TODO boost commonly matched words

            foreach (Match fullMatch in fullWordMatches)
            {
                var matchedWord = fullMatch.Groups[0];
                //var word = matchedWord.Value.Substring(Options.IndexSeperator.Length, matchedWord.Value.Length -1 - Options.IndexSeperator.Length * 2);

                var property = item.PropertyIndexMap.First(p => p.Value > matchedWord.Index);

                var option = Options.Properties[property.Key];

                if (CommonWords.Contains(matchedWord.Value))
                {                    
                    score += (option.Score * (matchedWord.Length) * (option.RequiresFullWordMatch ? 2 : 1)) / 5;
                }
                else
                {
                    commonWordsOnly = false;
                    score += (option.Score * (matchedWord.Length) * (option.RequiresFullWordMatch ? 2 : 1)) * 3;
                }

            }

            var partialWordMatches = partialMatchRegex.Matches(item.Index);

            // TODO, avoid rematching values from the fullmatch
            foreach (Match partialMatch in partialWordMatches)
            {
                var matchedWord = partialMatch.Groups[1];
                var property = item.PropertyIndexMap.First(p => p.Value > matchedWord.Index);
                
                var option = Options.Properties[property.Key];

                if (option.RequiresFullWordMatch)
                    continue;

                // TODO, matchedWord is unlikely to ever match as the value is a substring...
                if (CommonWords.Contains(matchedWord.Value))
                {
                    score += (option.Score * (matchedWord.Length) * 1) / 5;
                }
                else
                {
                    score += option.Score * (matchedWord.Length) * 1;
                }
            }

            return new Tuple<double, bool>(score, commonWordsOnly);

            //foreach (var indexedProperty in item.Properties)
            //{
            //    var option = Options.Properties[indexedProperty.Key];

            //    foreach (var searchWord in searchQuery)
            //    {
            //        var term = Options.IndexSeperatorChar + searchWord + Options.IndexSeperatorChar;
            //        double score = 0;

            //        if (indexedProperty.Value.Contains(term))
            //        {
            //            score = option.Score * (searchWord.Length) * (option.RequiresFullWordMatch ? 2 : 1);
            //        }else if(!option.RequiresFullWordMatch && indexedProperty.Value.Contains(term))
            //        {
            //            score = (option.Score * (searchWord.Length)) / 2;
            //        }

            //        if(score > 0)
            //        {
            //            // Increment the score value
            //            if (SearchProperties.ContainsKey(indexedProperty.Key))
            //                SearchProperties[indexedProperty.Key] += score;
            //            else
            //                SearchProperties.Add(indexedProperty.Key, score);
            //        }
            //    }
            //}

            //if (SearchProperties.Any())
            //{
            //    return new RankedResult<TType>
            //    {
            //        Item = item.Item,
            //        Properties = SearchProperties.ToDictionary(p=> p.Key, p=> p.Value)
            //    };
            //}
            //return null;
        }
    }
}
