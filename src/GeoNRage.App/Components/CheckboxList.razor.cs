using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components
{
    public partial class CheckboxList<T>
    {
        [Parameter]
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

        [Parameter]
        public Func<T, string> LabelSelector { get; set; } = null!;

        [Parameter]
        public Func<T, int> IdSelector { get; set; } = null!;

        [Parameter]
        [SuppressMessage("Usage", "CA2227", Justification = "Component parameter.")]
        public ICollection<int> SelectedIds { get; set; } = Array.Empty<int>();

        public void CheckboxClicked(int selectedId, object? value)
        {
            if ((bool?)value == true)
            {
                if (!SelectedIds.Contains(selectedId))
                {
                    SelectedIds.Add(selectedId);
                }
            }
            else
            {
                if (SelectedIds.Contains(selectedId))
                {
                    SelectedIds.Remove(selectedId);
                }
            }

            StateHasChanged();
        }
    }
}
