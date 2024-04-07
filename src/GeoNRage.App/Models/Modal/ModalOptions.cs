namespace GeoNRage.App.Models.Modal;

public record ModalOptions(bool IsCard, string? Title, bool IsSmall)
{
    public static ModalOptions Default => new(false, null, false);
}
