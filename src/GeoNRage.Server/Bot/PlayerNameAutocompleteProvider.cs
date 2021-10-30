using GeoNRage.Server.Services;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class PlayerNameAutocompleteProvider : IAutocompleteProvider
{
    private readonly PlayerService _playerService;

    /// <inheritdoc />
    public string Identity => nameof(PlayerNameAutocompleteProvider);

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync(
        IReadOnlyList<IApplicationCommandInteractionDataOption> options,
        string userInput,
        CancellationToken ct = default)
    {
        return (await _playerService.GetAllAsync())
            .Where(p => p.Name.RemoveDiacritics().Contains(userInput.RemoveDiacritics(), StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .Select(p => new ApplicationCommandOptionChoice(p.Name, p.Name))
            .ToList();
    }
}
