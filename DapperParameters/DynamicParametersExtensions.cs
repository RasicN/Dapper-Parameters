using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using DapperParameters.Attributes;

namespace DapperParameters
{
    public static class DynamicParametersExtensions
    {
        public static void AddTable<T>(this DynamicParameters source, string parameterName, string dataTableType, ICollection<T> values)
        {
            var table = new DataTable();

            var properties = typeof(T).GetRuntimeProperties().ToArray();

            foreach (var prop in properties)
            {
                if (IgnoreProperty(prop))
                {
                    // Property doesn't have a public setter or marked as ignore so let's ignore it
                    continue;
                }
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var value in values)
            {
                var parameters = new List<object>();
                var valueProperties = value.GetType().GetRuntimeProperties().ToArray();
                
                foreach (var prop in valueProperties)
                {
                    if (IgnoreProperty(prop))
                    {
                        // Property doesn't have a public setter or marked as ignore so let's ignore it
                        continue;
                    }
                    var p = prop.GetValue(value);
                    parameters.Add(p);
                }

                table.Rows.Add(parameters.ToArray());
            }

            source.Add(parameterName, table.AsTableValuedParameter(dataTableType));
        }

        private static bool IgnoreProperty(PropertyInfo prop)
        {
            return prop.SetMethod == null || GetAttribute<IgnorePropertyAttribute>(prop) != null;
        }

        private static T GetAttribute<T>(MemberInfo propInfo)
        {
            if (propInfo.GetCustomAttributes().FirstOrDefault(a => a is T) is T attribute)
            {
                return attribute;
            }

            return default;
        }
    }
}
