using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Players;

public partial class FilterPanel : IModal<IEnumerable<string>>
{
    private IEnumerable<string>? _tags;

    public string Id => nameof(FilterPanel);

    [Parameter]
    public IReadOnlyCollection<string>? Tags { get; set; }

    protected override void OnInitialized()
    {
        _tags = Tags;
    }

    public IEnumerable<string> Close()
    {
        return _tags ?? [];
    }

    public void OnTagsChanged(IEnumerable<string> tags)
    {
        _tags = tags;
    }
}
