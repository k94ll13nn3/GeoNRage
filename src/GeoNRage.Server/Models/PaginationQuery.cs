using System.ComponentModel;
using Gridify;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Models;

/// <summary>
/// Pagination query that will be used to paginate the filter results.
/// </summary>
/// <param name="page">The page of the result.</param>
/// <param name="pageSize">The page size of the result.</param>
/// <param name="filter">The filtering expression.</param>
/// <param name="orderBy">The ordering expression.</param>
internal sealed class PaginationQuery(int page = 1, int pageSize = 20, string? filter = null, string? orderBy = null) : IGridifyQuery, IPaginationQuery
{
    /// <summary>
    /// The page of the result.
    /// </summary>
    [DefaultValue(1)]
    [FromQuery(Name = "page")]
    [Description("The page of the result.")]
    public int Page { get; set; } = page;

    /// <summary>
    /// The page size of the result.
    /// </summary>
    [DefaultValue(20)]
    [FromQuery(Name = "pageSize")]
    [Description("The page size of the result.")]
    public int PageSize { get; set; } = pageSize;

    [FromQuery(Name = "filter")]
    [Description(@"The following filter operators are supported:

### Conditional Operators

| Name                  | Operator | Usage example        |
|-----------------------|----------|----------------------|
| Equal                 | `=`      | `FieldName = Value`  |
| NotEqual              | `!=`     | `FieldName !=Value`  |
| LessThan              | `<`      | `FieldName < Value`  |
| GreaterThan           | `>`      | `FieldName > Value`  |
| GreaterThanOrEqual    | `>=`     | `FieldName >=Value`  |
| LessThanOrEqual       | `<=`     | `FieldName <=Value`  |
| Contains - Like       | `=*`     | `FieldName =*Value`  |
| NotContains - NotLike | `!*`     | `FieldName !*Value`  |
| StartsWith            | `^`      | `FieldName ^ Value`  |
| NotStartsWith         | `!^`     | `FieldName !^ Value` |
| EndsWith              | `$`      | `FieldName $ Value`  |
| NotEndsWith           | `!$`     | `FieldName !$ Value` |

> Tip: If you don't specify any value after `=` or `!=` operators, the API searches for the `default` and `null` values.

### Logical Operators

| Name        | Operator | Usage example                                   |
|-------------|----------|-------------------------------------------------|
| AND         | `,`      | `FirstName = Value, LastName = Value2`          |
| OR          | `\|`     | `FirstName=Value\|LastName=Value2`              |
| Parenthesis | `()`     | `(FirstName=*Jo,Age<30)\|(FirstName!=Hn,Age>30)`|

### Case Insensitive Operator

The `/i` operator can be use after string values for case insensitive searches. You should only use this operator after the search value. 

Example:
```
FirstName=John/i
```

this query matches with `JOHN`, `john`, `John`, `jOHn`, etc.")]
    public string? Filter { get; set; } = filter;

    /// <summary>
    /// The ordering expression.
    /// </summary>
    [FromQuery(Name = "orderBy")]
    [Description(@"The ordering query expression can be built with a comma-delimited ordered list of field/property names, followed by `asc` or `desc` keywords. 

By default, if you don't add these keywords, the API assumes you need Ascending ordering.")]
    public string? OrderBy { get; set; } = orderBy;
}
