using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class Table<T>
    {
        [Parameter]
        public IEnumerable<T> Items { get; set; } = null!;

        [Parameter]
        public RenderFragment<T> RowContent { get; set; } = null!;

        [Parameter]
        public RenderFragment? FooterContent { get; set; }

        [Parameter]
        public IEnumerable<string> Headers { get; set; } = null!;
    }
}
