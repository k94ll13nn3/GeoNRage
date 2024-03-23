using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class CheckboxList<TItem, TId>
{
    [Parameter]
    public IEnumerable<TItem> Data { get; set; } = [];

    [Parameter]
    public Func<TItem, string> LabelSelector { get; set; } = null!;

    [Parameter]
    public Func<TItem, TId> IdSelector { get; set; } = null!;

    [Parameter]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Blazor parameters must have a setter")]
    public ICollection<TId> SelectedIds { get; set; } = [];

    [Parameter]
    public string Label { get; set; } = null!;

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
            SelectedIds.Remove(selectedId);
        }

        StateHasChanged();
    }
}
