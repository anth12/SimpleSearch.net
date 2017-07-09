using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleSearch
{
    public class SearchIndexOptions<TType>
    {
        internal Dictionary<string, SearchIndexOption> Properties { get; set; } 
                    = new Dictionary<string, SearchIndexOption>();

        public SearchIndexOptions<TType> AddProperty(Expression<Func<TType, string>> propertyLambda, double rank = 0.8d, bool requiresFullWordMatch = false)
        {
            Type type = typeof(TType);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda.ToString()}' refers to a method, not a property.");

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda.ToString()}' refers to a field, not a property.");


            Properties.Add(propInfo.ToString(), new SearchIndexOption
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
