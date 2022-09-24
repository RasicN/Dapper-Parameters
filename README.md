# Dapper-Parameters
Project to extend on Dynamic Paramters for Dapper.  Currently only has a method to handle User-Defined Table Types.

## Example
Given that we have a User-Defined Table Type of: `[dbo].[IntList]` that is defined as such:
```sql
CREATE TYPE [dbo].[IntList] AS TABLE(
	[IntValue] [int] NOT NULL
)
```
All we need is to have a csharp class that is defined the same way as our type (similar to the way Dapper maps query results):
```csharp
// Class Name doesn't matter
public class IntListType 
{
  // Must match Table Type name!
  public int IntValue { get; set; }
}
```

Then put it all together:
```csharp
var ids = new List<int>{1,2,3,4};
var intList = ids.Select(x => new IntListType { IntValue = x }).ToList();

var parameters = new DynamicParameters();
parameters.AddTable("@sprocParameterName", "[dbo].[IntList]", intList);

connection.Execute("storedProcedure", parameters, CommandType.StoredProcedure);
```

###If you have multipe values in your table type such as:

```sql
CREATE TYPE [dbo].[TestType] AS TABLE(
	[TestValue1] [int] NOT NULL,
  [TestValue2] [nvarchar(50)] NOT NULL
)
```
Your matching C# class properties must match exact order:

**GOOD**
```csharp
public class TestType 
{
  public int TestValue1 { get; set; }
  public string TestValue2 { get; set; }
}
```

**BAD**
```csharp
public class TestType 
{
  public string TestValue2 { get; set; } // This comes second in your TestType sql definition so it needs to be second in this class
  public int TestValue1 { get; set; }
}
```

*Does not support anything other than flat types, so no nested types
