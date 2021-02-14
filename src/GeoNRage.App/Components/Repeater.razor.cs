using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components
{
    public partial class Repeater<T>
    {
        [Parameter]
        public IEnumerable<T>? Items { get; set; }

        [Parameter]
        public RenderFragment<T>? ChildContent { get; set; }
    }
}
