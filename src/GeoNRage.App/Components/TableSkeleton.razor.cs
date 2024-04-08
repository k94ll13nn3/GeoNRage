using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class TableSkeleton
{
    [Parameter]
    public bool Paginate { get; set; }

    [Parameter]
    public bool CanSearch { get; set; }
}
