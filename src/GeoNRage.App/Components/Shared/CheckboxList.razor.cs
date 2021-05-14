using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared
{
    public partial class CheckboxList<TItem, TId>
    {
        [Parameter]
        public IEnumerable<TItem> Data { get; set; } = Enumerable.Empty<TItem>();

        [Parameter]
        public Func<TItem, string> LabelSelector { get; set; } = null!;

        [Parameter]
        public Func<TItem, TId> IdSelector { get; set; } = null!;

        [Parameter]
        [SuppressMessage("Usage", "CA2227", Justification = "Component parameter.")]
        public ICollection<TId> SelectedIds { get; set; } = Array.Empty<TId>();

        public void CheckboxClicked(TId selectedId, object? value)
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
