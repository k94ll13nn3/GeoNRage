using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Players;

public partial class FilterPanel : IModal<IEnumerable<string>>
{
    private IEnumerable<string>? _tags;

    [Parameter]
    public IReadOnlyCollection<string>? Tags { get; set; }

    protected override void OnInitialized()
    {
        _tags = Tags;
    }

    public IEnumerable<string> Close()
    {
        return _tags ?? new List<string>();
    }

    public void OnTagsChanged(IEnumerable<string> tags)
    {
        _tags = tags;
    }
}
