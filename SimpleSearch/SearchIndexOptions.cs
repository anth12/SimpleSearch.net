using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleSearch
{
    public class SearchIndexOptions<TType>
    {
        internal Dictionary<string, Tuple<double, Func<TType, string>>> Properties { get; set; } 
                    = new Dictionary<string, Tuple<double, Func<TType, string>>>();

        public SearchIndexOptions<TType> AddProperty(Expression<Func<TType, string>> propertyLambda, double rank = 0.8d, bool requiresFullMatch = false)
        {
            Type type = typeof(TType);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda.ToString()}' refers to a method, not a property.");

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda.ToString()}' refers to a field, not a property.");


            Properties.Add(propInfo.ToString(), new Tuple<double, Func<TType, string>>(rank, propertyLambda.Compile()));
            
            return this;
        }

    }
}
