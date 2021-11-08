using GeoNRage.Server.Services;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class MapNameAutocompleteProvider : IAutocompleteProvider
{
    private readonly MapService _mapService;

    public string Identity => nameof(MapNameAutocompleteProvider);

    public async ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync(
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default)
    {
        return (await _mapService.GetAllAsync())
            .Where(m => m.Name.RemoveDiacritics().Contains(userInput.RemoveDiacritics(), StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(m => m.IsMapForGame)
            .Select(m => new ApplicationCommandOptionChoice(m.Name, m.Id))
            .ToList();
    }
}
