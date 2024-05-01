namespace GeoNRage.App.Models.Modal;

public record ModalOptions(ModalSize Size, bool ShowCloseButton)
{
    public static ModalOptions Default => new(ModalSize.Medium, false);
}
