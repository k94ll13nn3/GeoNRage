using GeoNRage.App.Layouts.Main;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class LoaderModal : IModal
{
    public string Id => nameof(LoaderModal);

    [Parameter]
    public Func<Task> Action { get; set; } = null!;

    [CascadingParameter]
    public ModalRender ModalRender { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Action is not null)
            {
                await Action();
            }

            ModalRender.Close();
        }
    }
}
