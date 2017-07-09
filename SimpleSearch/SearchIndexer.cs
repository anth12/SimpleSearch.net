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

            return new SearchIndex<TType>
            {
                Options = options,
                Source = source.Select(c=> IndexItem(c, options)).ToList()
            };
        }

        public static SearchIndex<TType> ReBuild<TType>(SearchIndex<TType> original, ObservableCollection<TType> source)
        {
            return original;
        }

        public static SearchIndex<TType> ReBuild<TType>(SearchIndex<TType> original, ObservableCollection<TType> source, SearchIndexOptions<TType> options)
        {
            return original;
        }

        private static SearchItem<TType> IndexItem<TType>(TType item, SearchIndexOptions<TType> options)
        {
            var index = new SearchItem<TType>(item);

            foreach(var property in options.Properties)
            {
                var cleanValues = property.Value.Item2.Invoke(item)?.Split(' ').Select(x => x.Clean()).ToArray();

                if(cleanValues != null && cleanValues.Any())
                    index.Properties.Add(property.Key, cleanValues);
            }

            return index;
        }
    }
}
