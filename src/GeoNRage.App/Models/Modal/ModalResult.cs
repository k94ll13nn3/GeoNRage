using System.Diagnostics.CodeAnalysis;

namespace GeoNRage.App.Models.Modal;

public sealed record ModalResult<T>(T? Value, [property: MemberNotNullWhen(false, "Value")] bool Cancelled);

public sealed record ModalResult(bool Cancelled);
