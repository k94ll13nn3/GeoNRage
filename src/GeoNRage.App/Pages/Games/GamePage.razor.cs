using System.Security.Claims;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;

namespace GeoNRage.App.Pages.Games;

public partial class GamePage : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationToken = new();
    private HubConnection _hubConnection = null!;

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

    public bool GameFound { get; set; } = true;

    public GameDetailDto Game { get; set; } = null!;

    public bool Loaded { get; set; }

    public bool ShowRankings { get; set; }

    public bool ShowChart { get; set; }

    public Dictionary<(int challengeId, string playerId, int round), int?> Scores { get; } = new();

    public ClaimsPrincipal User { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        _cancellationToken.Cancel();
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
        Loaded = false;
        await OnInitializedAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<int, string, int, int>("ReceiveValue", HandleReceiveValue);
        _hubConnection.On("NewPlayerAdded", () => ToastService.DisplayToast("Un nouveau joueur a été ajouté à la partie. Veuillez rafraichir la page pour voir ses scores.", null, ToastType.Information, "toast-new-player"));
        _hubConnection.On("Taunted", () => ToastService.DisplayToast(TauntFragment, TimeSpan.FromMilliseconds(2000), ToastType.Error, "toast-taunt"));

        _hubConnection.Closed += OnHubConnectionClosed;
        _hubConnection.Reconnecting += OnHubConnectionReconnecting;
        _hubConnection.Reconnected += OnHubConnectionReconnected;

        await _hubConnection.StartAsync(_cancellationToken.Token);

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

            await _hubConnection.InvokeAsync("JoinGroup", Id, _cancellationToken.Token);
            User = (await AuthenticationState).User;
            StateHasChanged();
        }
    }

    private Task OnHubConnectionReconnected(string? arg)
    {
        ToastService.DisplayToast(
            "Connexion avec le serveur rétablie. Vous pouvez à nouveau envoyer et recevoir des données, cependant les données reçues pendant la tentative de reconnexion ne seront pas mise à jour.",
            null,
            ToastType.Information,
            "signalr-connection",
            true);
        return Task.FromResult(0);
    }

    private Task OnHubConnectionReconnecting(Exception? arg)
    {
        ToastService.DisplayToast(
            "Connexion avec le serveur perdue, tentative de reconnexion...",
            null,
            ToastType.Warning,
            "signalr-connection",
            true);
        return Task.FromResult(0);
    }

    private Task OnHubConnectionClosed(Exception? arg)
    {
        ToastService.DisplayToast(
            "Connexion avec le serveur perdue, reconnexion echouée.",
            null,
            ToastType.Error,
            "signalr-connection",
            true);
        return Task.FromResult(0);
    }

    private void HandleReceiveValue(int challengeId, string playerId, int round, int score)
    {
        Scores[(challengeId, playerId, round)] = score;
        StateHasChanged();
    }

    private async Task SendAsync(int challengeId, int round, int? score)
    {
        if (User.PlayerId() is not string playerId)
        {
            return;
        }

        int clampedValue = Math.Clamp(score ?? 0, 0, 5000);
        HandleReceiveValue(challengeId, playerId, round, clampedValue);
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
