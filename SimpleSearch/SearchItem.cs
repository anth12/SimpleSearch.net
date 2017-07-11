
using System.Collections.Generic;

namespace SimpleSearch
{
    internal struct SearchItem<TType>
    {
        public SearchItem(TType item)
        {
            Item = item;
            Properties = new Dictionary<string, string[]>();
        }

        public Dictionary<string, string[]> Properties { get; set; }
        public TType Item { get; set; }

    }
}
