using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class Container
    {
        [Parameter]
        public bool Condition { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
