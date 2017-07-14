using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimpleSearch
{
    [DebuggerDisplay("{Item}: {Index}")]
    internal struct SearchItem<TType>
    {
        public SearchItem(TType item)
        {
            Item = item;
            Index = "";
            PropertyIndexMap = new Dictionary<string, int>();

        }

        /// <summary>
        /// Maps the end index of Properties stored in the Index
        /// </summary>
        public Dictionary<string, int> PropertyIndexMap;
        public string Index;
        public TType Item;

        public override string ToString()
        {
            var result = new StringBuilder();

            var startIndex = 0;
            foreach(var prop in PropertyIndexMap)
            {
                result.AppendLine($"{prop.Key}: {Index.Substring(startIndex, prop.Value)}");
                startIndex = prop.Value + 1;
            }

            return result.ToString();
        }
    }
}
