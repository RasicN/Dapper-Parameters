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
        public static void AddTable<T>(this DynamicParameters source, string parameterName, string dataTableType, IEnumerable<T> values)
        {
            var table = new DataTable();
            // Get Properties in order of declaration and ignore if doesn't have a public setter or marked as ignore
            var properties = GetOrderedProperties<T>().Where(property => !IgnoreProperty(property)).ToList();

            foreach (var prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var value in values)
            {
                table.Rows.Add(properties.Select(property => property.GetValue(value)).ToArray());
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

        private static IEnumerable<PropertyInfo> GetOrderedProperties<T>()
        {
            return typeof(T)
                .GetRuntimeProperties()
                .OrderBy(propertyInfo => GetAttribute<OrderAttribute>(propertyInfo)?.Order ?? int.MaxValue);
        }
    }
}
