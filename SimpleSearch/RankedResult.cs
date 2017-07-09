using System.Collections.Generic;
using System.Linq;

namespace SimpleSearch
{
    public class RankedResult<TType>
    {
        public TType Item { get; set; }

        public Dictionary<string, double> Properties { get; set; } = new Dictionary<string, double>();

        public double Score => Properties.Sum(p => p.Value);
    }
}
