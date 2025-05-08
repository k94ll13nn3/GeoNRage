using GeoNRage.App.Apis;
using GeoNRage.App.Layouts.Main;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class GameAdminCreateOrEdit : IModal
{
    private GameCreateOrEditDto _game = new();
    private EditForm _form = null!;
    private string? _error;
    private IReadOnlyCollection<MapDto> _maps = [];
    private IReadOnlyCollection<PlayerDto> _players = [];

    [CascadingParameter]
    public ModalRender ModalRender { get; set; } = null!;

    [Parameter]
    public GameAdminViewDto? Game { get; set; }

    [Inject]
    public IGamesApi GamesApi { get; set; } = null!;

    [Inject]
    public IMapsApi MapsApi { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    public string Id => nameof(GameAdminCreateOrEdit);

    protected override async Task OnInitializedAsync()
    {
        if (Game is not null)
        {
            _game = new GameCreateOrEditDto
            {
                Name = Game.Name,
                Date = Game.Date,
                Challenges = [.. Game.Challenges.Select(c => new GameChallengeCreateOrEditDto { Id = c.Id, GeoGuessrId = c.GeoGuessrId, MapId = c.MapId })],
                PlayerIds = [.. Game.PlayerIds]
            };
        }
        else
        {
            _game = new GameCreateOrEditDto { Date = DateTime.Now };
        }

        _maps = [.. (await MapsApi.GetAllAsync()).OrderByDescending(m => m.IsMapForGame)];
        _players = await PlayersApi.GetAllAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        _form?.EditContext?.UpdateCssClassProvider();
    }

    private void Cancel()
    {
        ModalRender.Cancel();
    }

    private async Task CreateOrUpdateGameAsync()
    {
        try
        {
            if (Game is not null)
            {
                await GamesApi.UpdateAsync(Game.Id, _game);
            }
            else
            {
                await GamesApi.CreateAsync(_game);
            }
            ModalRender.Close();
        }
        catch (ApiException e)
        {
            _error = $"Error: {(await e.GetContentAsAsync<ProblemDetails>())?.Detail}";
        }
    }
}
