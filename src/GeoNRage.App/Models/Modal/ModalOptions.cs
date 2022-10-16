namespace GeoNRage.App.Models.Modal;

public record ModalOptions(bool IsCard, string? Title)
{
    public static ModalOptions Default => new(false, null);
}
