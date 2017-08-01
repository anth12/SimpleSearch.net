using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SimpleSearch
{
    public class SearchIndexer
    {
        public static SearchIndex<TType> Build<TType>(IEnumerable<TType> source)
        {
            var options = new SearchIndexOptions<TType>();
            return Build(source, options);
        }

        public static SearchIndex<TType> Build<TType>(IEnumerable<TType> source, SearchIndexOptions<TType> options)
        {
            if (!options.Properties.Any())
                throw new Exception($"No properties specified to index");

            var wordFrequency = new Dictionary<string, int>();
            var index =  new SearchIndex<TType>
            {
                Options = options,
                Source = source.Select(c=> IndexItem(c, options, wordFrequency)).ToList()
            };

            // Index the top 1% of common words to give low scores too
            var possibleCommonWords = wordFrequency.Where(w => w.Value > 2);
            index.CommonWords = possibleCommonWords.OrderByDescending(c => c.Value).Take(possibleCommonWords.Count() / 100).Select(w => w.Key).ToArray();
                        
            return index;
        }
        
        public static SearchIndex<TType> ReBuild<TType>(SearchIndex<TType> original, ObservableCollection<TType> source)
        {
            return original;
        }

        public static SearchIndex<TType> ReBuild<TType>(SearchIndex<TType> original, ObservableCollection<TType> source, SearchIndexOptions<TType> options)
        {
            return original;
        }

        private static SearchItem<TType> IndexItem<TType>(TType item, SearchIndexOptions<TType> options, Dictionary<string, int> wordFrequency)
        {
            var index = new SearchItem<TType>(item);

            foreach(var property in options.Properties)
            {
                var propertyValues = property.Value.Func.Invoke(item)?.Split(' ').Select(x => x.Clean());

                if (propertyValues == null || !propertyValues.Any())
                    continue;

                foreach(var word in propertyValues)
                {
                    if (word.Length > 2)
                    {
                        if (wordFrequency.ContainsKey(word))
                            wordFrequency[word]++;
                        else
                            wordFrequency.Add(word, 1);
                    }
                }

                var propertyIndexValue = string.Join(options.IndexSeperator, propertyValues);

                if (!string.IsNullOrEmpty(propertyIndexValue))
                {
                    index.Index += $"{options.IndexSeperator}{propertyIndexValue}{options.IndexSeperator}";

                    index.PropertyIndexMap.Add(property.Key, index.Index.Length -1);

                }
            }

            return index;
        }
        
    }
}
