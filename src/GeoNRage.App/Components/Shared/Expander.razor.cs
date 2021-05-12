using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class Expander
    {
        [Parameter]
        public bool Expanded { get; set; } = true;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public bool IsExpanded { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            IsExpanded = Expanded;
        }
    }
}
