
using System.Collections.Generic;

namespace SimpleSearch
{
    internal class SearchItem<TType>
    {
        public SearchItem(TType item)
        {
            Item = item;
        }

        public Dictionary<string, string[]> Properties { get; set; } = new Dictionary<string, string[]>();
        public TType Item { get; set; }

    }
}
