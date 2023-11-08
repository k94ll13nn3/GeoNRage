using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace GeoNRage.App.Components;

public partial class TagsList
{
    private readonly List<string> _tags = new();

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = null!;

    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<IEnumerable<string>> TagsChanged { get; set; }

    [Parameter]
    public IReadOnlyCollection<string>? Tags { get; set; }

    public IJSObjectReference JSModule { get; set; } = null!;

    public bool IsFocused { get; set; }

    public string Value { get; set; } = string.Empty;

    public ElementReference EditableContent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Tags is not null)
        {
            _tags.AddRange(Tags);
        }

        JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./Components/{nameof(TagsList)}.razor.js");
        await JSModule.InvokeVoidAsync("addPreventDefault", EditableContent);
        await JSModule.InvokeVoidAsync("focusElement", EditableContent);
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs args)
    {
        if (new[] { "Enter", " ", ",", "Tab" }.Contains(args.Key))
        {
            if (ValidateTag(Value))
            {
                await AddTagAsync(Value);
                Value = "";
            }
        }
        else if (args.Key == "Backspace" && string.IsNullOrWhiteSpace(Value) && _tags.Count > 1)
        {
            await RemoveTagAsync(_tags[^1]);
        }
    }

    private async Task AddTagAsync(string tag)
    {
        _tags.Add(tag);
        await TagsChanged.InvokeAsync(_tags);
        StateHasChanged();
    }

    private async Task RemoveTagAsync(string tag)
    {
        _tags.Remove(tag);
        await TagsChanged.InvokeAsync(_tags);
        StateHasChanged();
    }

    private bool ValidateTag(string tag)
    {
        return tag.Length >= 5 && tag.Length <= 50 && !_tags.Contains(tag);
    }
}
