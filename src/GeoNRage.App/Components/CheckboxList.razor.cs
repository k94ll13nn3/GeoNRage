using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class CheckboxList<TItem, TId>
{
    [Parameter]
    public IEnumerable<TItem> Data { get; set; } = Enumerable.Empty<TItem>();

    [Parameter]
    public Func<TItem, string> LabelSelector { get; set; } = null!;

    [Parameter]
    public Func<TItem, TId> IdSelector { get; set; } = null!;

    [Parameter]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227", Justification = "Setter needed as it is a parameter")]
    public ICollection<TId> SelectedIds { get; set; } = Array.Empty<TId>();

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
        else if (SelectedIds.Contains(selectedId))
        {
            SelectedIds.Remove(selectedId);
        }

        StateHasChanged();
    }
}
