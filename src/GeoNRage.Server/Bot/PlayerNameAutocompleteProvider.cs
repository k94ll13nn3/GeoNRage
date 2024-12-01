using GeoNRage.Server.Services;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
internal sealed partial class PlayerNameAutocompleteProvider : IAutocompleteProvider
{
    private readonly PlayerService _playerService;

    public string Identity => nameof(PlayerNameAutocompleteProvider);

    public async ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync(
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default)
    {
        return (await _playerService.GetAllAsync())
            .Where(p => p.Name.RemoveDiacritics().Contains(userInput.RemoveDiacritics(), StringComparison.OrdinalIgnoreCase))
            .Select(p => new ApplicationCommandOptionChoice(p.Name, p.Id))
            .ToList();
    }
}
