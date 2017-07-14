using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace SimpleSearch
{
    public class SearchIndexOptions<TType>
    {
        /// <summary>
        /// Creates options used to index a collection
        /// </summary>
        /// <param name="indexSeperator">Seperator string used internally to seperate words within an index for each property</param>
        /// <param name="resultThreshold">Percentage value (0d-1d) the matched search results must meet in comparison to the highest ranking result</param>
        public SearchIndexOptions(string indexSeperator = "|", double resultThreshold = 0.3d)
        {
            IndexSeperator = indexSeperator;
            RegexSafeIndexSeperatorChar = @"\" + string.Join(@"\", indexSeperator.ToArray());
            ResultThreshold = resultThreshold;
        }
        

        /// <summary>
        /// Seperator string used internally to seperate words within an index for each property
        /// </summary>
        internal string IndexSeperator;

        /// <summary>
        /// A Regex safe (escaped) version of <see cref="IndexSeperator"/>
        /// </summary>
        internal string RegexSafeIndexSeperatorChar;

        /// <summary>
        /// Percentage value (0d-1d) the matched search results must meet in comparison to the highest ranking result
        /// </summary>
        internal double ResultThreshold;

        /// <summary>
        /// Index Dictionary of all searchable properties in the model
        /// </summary>
        internal Dictionary<string, SearchIndexOption> Properties 
                    = new Dictionary<string, SearchIndexOption>();

        /// <summary>
        /// Backing field used to lazy initialise the <see cref="FullMatchProperties"/>
        /// </summary>
        private string _fullMatchProperties;

        internal string FullMatchProperties => _fullMatchProperties != null
            ? _fullMatchProperties
            : (_fullMatchProperties = string.Join("|", Properties.Where(p => p.Value.RequiresFullWordMatch).Select(p => p.Key)));

        /// <summary>
        /// Backing field used to lazy initialise the <see cref="PartialMatchProperties"/>
        /// </summary>
        private string _partialMatchProperties;

        internal string PartialMatchProperties => _partialMatchProperties != null
            ? _partialMatchProperties
            : (_partialMatchProperties = string.Join("|", Properties.Where(p => !p.Value.RequiresFullWordMatch).Select(p => p.Key)));

        /// <summary>
        /// Adds a property that is Index'able
        /// </summary>
        /// <param name="propertyLambda">Property selector</param>
        /// <param name="rank">Porperty score value, adds weight to matches</param>
        /// <param name="requiresFullWordMatch">
        /// When true, search terms must match exactly in order to score. Otherwise partial matches are scored (with a lower value)
        /// </param>
        public SearchIndexOptions<TType> AddProperty(Expression<Func<TType, string>> propertyLambda, double rank = 0.8d, bool requiresFullWordMatch = false)
        {
            Type type = typeof(TType);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda.ToString()}' refers to a method, not a property.");

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda.ToString()}' refers to a field, not a property.");


            Properties.Add(propInfo.Name, new SearchIndexOption
            {
                Score = rank,
                Func = propertyLambda.Compile(),
                RequiresFullWordMatch = requiresFullWordMatch
            });
            
            return this;
        }

        internal struct SearchIndexOption
        {
            internal bool RequiresFullWordMatch;
            internal Func<TType, string> Func;
            internal double Score;
        }
    }

}
