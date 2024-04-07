using GeoNRage.App.Layouts.Main;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Players;

public partial class FilterPanel : IModal
{
    private IEnumerable<string>? _tags;

    public string Id => nameof(FilterPanel);

    [Parameter]
    public IReadOnlyCollection<string>? Tags { get; set; }

    [CascadingParameter]
    public ModalRender ModalRender { get; set; } = null!;

    protected override void OnInitialized()
    {
        _tags = Tags;
    }

    public void OnTagsChanged(IEnumerable<string> tags)
    {
        _tags = tags;
    }

    private void Confirm()
    {
        ModalRender.Close(_tags);
    }
}
