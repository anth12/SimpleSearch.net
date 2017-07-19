using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimpleSearch
{
    [DebuggerDisplay("Score: {Score}; {Properties.First().Key}: {Properties.First().Value}")]
    public class RankedResult<TType>
    {
        public TType Item;

        public Dictionary<string, double> Properties = new Dictionary<string, double>();

        public double Score => Properties.Sum(p => p.Value);
    }
}
