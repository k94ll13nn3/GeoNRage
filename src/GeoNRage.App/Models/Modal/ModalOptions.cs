namespace GeoNRage.App.Models.Modal;

public record ModalOptions(bool IsSmall, bool ShowCloseButton)
{
    public static ModalOptions Default => new(false, false);
}
