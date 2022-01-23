using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace GeoNRage.App.Components;

public partial class TagsList
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; } = null!;

    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<IEnumerable<string>> TagsChanged { get; set; }

    public IJSObjectReference JSModule { get; set; } = null!;

    public IList<string> Tags { get; } = new List<string>();

    public bool IsFocused { get; set; }

    public string Value { get; set; } = string.Empty;

    public ElementReference EditableContent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./Components/{nameof(TagsList)}.razor.js");
        await JSModule.InvokeVoidAsync("addPreventDefault", EditableContent);
    }

    private void HandleOnKeyDown(KeyboardEventArgs args)
    {
        if (new[] { "Enter", " ", ",", "Tab" }.Contains(args.Key))
        {
            if (ValidateTag(Value))
            {
                AddTag(Value);
                Value = "";
            }
        }
        else if (args.Key == "Backspace" && string.IsNullOrWhiteSpace(Value) && Tags.Count > 1)
        {
            RemoveTag(Tags[^1]);
        }
    }

    private void AddTag(string tag)
    {
        Tags.Add(tag);
        TagsChanged.InvokeAsync(Tags);
        StateHasChanged();
    }

    private void RemoveTag(string tag)
    {
        Tags.Remove(tag);
        TagsChanged.InvokeAsync(Tags);
        StateHasChanged();
    }

    private bool ValidateTag(string tag)
    {
        return tag.Length >= 5 && tag.Length <= 50 && !Tags.Contains(tag);
    }
}
