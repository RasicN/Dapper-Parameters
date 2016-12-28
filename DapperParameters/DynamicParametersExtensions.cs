using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;

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
                if (prop.SetMethod == null)
                {
                    // Property doesn't have a public setter so let's ignore it
                    continue;
                }
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var value in values)
            {
                var parameters = new List<object>();
                var valueProperties = value.GetType().GetRuntimeProperties().ToArray();
                
                foreach (var prop in valueProperties)
                {
                    var i = 0;
                    if (prop.SetMethod == null)
                    {
                        // Property doesn't have a public setter so let's ignore it
                        continue;
                    }
                    var p = prop.GetValue(value);
                    parameters.Add(p);
                }

                table.Rows.Add(parameters.ToArray());
            }

            source.Add(parameterName, table.AsTableValuedParameter(dataTableType));
        }
    }
}
