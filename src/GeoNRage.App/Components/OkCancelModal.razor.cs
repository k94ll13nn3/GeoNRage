using GeoNRage.App.Layouts.Main;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components;

public partial class OkCancelModal : IModal
{
    public string Id => nameof(OkCancelModal);

    [Parameter]
    public string Message { get; set; } = null!;

    [Parameter]
    public string Title { get; set; } = null!;

    [CascadingParameter]
    public ModalRender ModalRender { get; set; } = null!;

    private void Confirm()
    {
        ModalRender.Close();
    }

    private void Cancel()
    {
        ModalRender.Cancel();
    }
}
