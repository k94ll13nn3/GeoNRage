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
        if (!string.IsNullOrWhiteSpace(Value))
        {
            foreach (string tag in Value.Split(','))
            {
                Tags.Add(tag);
            }
        }
    }

    private async Task HandleOnClickAsync()
    {
        await EditableContent.FocusAsync();
    }

    private void HandleOnBlur()
    {
        IsFocused = false;
    }

    private void HandleOnFocus()
    {
        IsFocused = true;
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs args)
    {
        string value = (await JSModule.InvokeAsync<string>("getTextContent", EditableContent)).Trim(',').Trim();

        if (new[] { "Enter", " ", ",", "Tab" }.Contains(args.Key))
        {
            if (ValidateTag(value))
            {
                AddTag(value);
                await JSModule.InvokeVoidAsync("resetElement", EditableContent);
                Value = string.Join(',', Tags);
            }
        }
        else if (args.Key == "Backspace" && string.IsNullOrWhiteSpace(value) && Tags.Count > 1)
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
