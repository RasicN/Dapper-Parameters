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

        public static void AddTable<T>(this DynamicParameters source, 
            string parameterName, 
            string dataTableType, 
            IEnumerable<T> values)
        {
            var table = new DataTable();

            var properties = typeof(T).GetRuntimeProperties();

            foreach (var prop in properties)
            {
                if (!IgnoreProperty(prop))
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            

            foreach (var value in values)
            {
                var parameters = properties
                            .Where(prop => !IgnoreProperty(prop))
                            .Select(prop => prop.GetValue(value));

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
