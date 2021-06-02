using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class NumericInput
    {
        private int? _value;

        public int? Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged.InvokeAsync(Value);
            }
        }

        [Parameter]
        public int? InitialValue { get; set; }

        [Parameter]
        public int TabIndex { get; set; } = 1;

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public EventCallback<int?> ValueChanged { get; set; }

        [Parameter]
        public string Class { get; set; } = string.Empty;

        protected override void OnParametersSet()
        {
            _value = InitialValue;
        }
    }
}
