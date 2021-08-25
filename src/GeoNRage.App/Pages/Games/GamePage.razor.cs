using System.Security.Claims;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;

namespace GeoNRage.App.Pages.Games;

public partial class GamePage : IAsyncDisposable
{
    private HubConnection _hubConnection = null!;

    public GameDetailDto Game { get; set; } = null!;

    [Parameter]
    public int Id { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public bool HideScores { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    [Inject]
    public IGamesApi GamesApi { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    public GameChart? Chart { get; set; } = null!;

    public bool GameFound { get; set; } = true;

    public bool Loaded { get; set; }

    public bool HubClosed { get; set; }

    public bool HubReconnecting { get; set; }

    public bool HubReconnected { get; set; }

    public bool ShowRankings { get; set; }

    public bool ShowChart { get; set; }

    public Dictionary<(int challengeId, string playerId, int round), int?> Scores { get; } = new();

    public ClaimsPrincipal User { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        _hubConnection.Closed -= OnHubConnectionClosed;
        _hubConnection.Reconnecting -= OnHubConnectionReconnecting;
        _hubConnection.Reconnected -= OnHubConnectionReconnected;
        await _hubConnection.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async Task ReloadPageAsync()
    {
        ShowChart = false;
        ShowRankings = false;
        HubClosed = false;
        HubReconnected = false;
        HubReconnecting = false;
        Loaded = false;
        await OnInitializedAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<int, string, int, int>("ReceiveValue", HandleReceiveValueAsync);
        _hubConnection.On("NewPlayerAdded", () => ToastService.DisplayToast("Un nouveau joueur a été ajouté à la partie. Veuillez rafraichir la page pour voir ses scores.", null, ToastType.Information, "toast-new-player"));
        _hubConnection.On("Taunted", () => ToastService.DisplayToast(TauntFragment, TimeSpan.FromMilliseconds(2000), ToastType.Error, "toast-taunt"));

        _hubConnection.Closed += OnHubConnectionClosed;
        _hubConnection.Reconnecting += OnHubConnectionReconnecting;
        _hubConnection.Reconnected += OnHubConnectionReconnected;

        await _hubConnection.StartAsync();

        ApiResponse<GameDetailDto> response = await GamesApi.GetAsync(Id);
        if (!response.IsSuccessStatusCode || response.Content is null)
        {
            GameFound = false;
        }
        else
        {
            Loaded = true;
            GameFound = true;
            Game = response.Content;
            foreach (GameChallengeDto challenge in Game.Challenges)
            {
                foreach (GamePlayerDto player in Game.Players)
                {
                    GameChallengePlayerScoreDto? playerScore = challenge.PlayerScores.FirstOrDefault(p => p.PlayerId == player.Id);
                    Scores[(challenge.Id, player.Id, 1)] = playerScore?.Round1;
                    Scores[(challenge.Id, player.Id, 2)] = playerScore?.Round2;
                    Scores[(challenge.Id, player.Id, 3)] = playerScore?.Round3;
                    Scores[(challenge.Id, player.Id, 4)] = playerScore?.Round4;
                    Scores[(challenge.Id, player.Id, 5)] = playerScore?.Round5;
                }
            }

            await _hubConnection.InvokeAsync("JoinGroup", Id);
            User = (await AuthenticationState).User;
            StateHasChanged();
        }
    }

    private Task OnHubConnectionReconnected(string? arg)
    {
        HubClosed = false;
        HubReconnected = true;
        HubReconnecting = false;
        StateHasChanged();
        return Task.FromResult(0);
    }

    private Task OnHubConnectionReconnecting(Exception? arg)
    {
        HubClosed = false;
        HubReconnected = false;
        HubReconnecting = true;
        StateHasChanged();
        return Task.FromResult(0);
    }

    private Task OnHubConnectionClosed(Exception? arg)
    {
        HubClosed = true;
        HubReconnected = false;
        HubReconnecting = false;
        StateHasChanged();
        return Task.FromResult(0);
    }

    private async Task HandleReceiveValueAsync(int challengeId, string playerId, int round, int score)
    {
        Scores[(challengeId, playerId, round)] = score;
        if (Chart is not null)
        {
            await Chart.UpdateAsync(playerId);
        }

        StateHasChanged();
    }

    private async Task SendAsync(int challengeId, int round, int? score)
    {
        if (User.PlayerId() is not string playerId)
        {
            return;
        }

        int clampedValue = Math.Clamp(score ?? 0, 0, 5000);
        await HandleReceiveValueAsync(challengeId, playerId, round, clampedValue);
        if (clampedValue == 5000)
        {
            ToastService.DisplayToast("5000 ! Quel talent !", TimeSpan.FromMilliseconds(2500), ToastType.Success, "toast-5000");
        }

        if (clampedValue == 4999)
        {
            ToastService.DisplayToast(SoCloseFragment, TimeSpan.FromMilliseconds(2500), ToastType.Warning, "toast-4999");
        }

        await _hubConnection.InvokeAsync("UpdateValue", Id, challengeId, User.PlayerId(), round, clampedValue);
    }

    private async Task AddPlayerAsync()
    {
        if (Game is not null && User.PlayerId() is string playerId && !Game.Players.Any(p => p.Id == User.PlayerId()))
        {
            await GamesApi.AddPlayerAsync(Game.Id, playerId);
            await _hubConnection.InvokeAsync("NotifyNewPlayer", Id);
            await ReloadPageAsync();
        }
    }

    private async Task TauntAsync(string playerId)
    {
        await _hubConnection.InvokeAsync("TauntPlayer", Id, playerId);
    }
}
