using GeoNRage.App.Apis;
using GeoNRage.App.Components.Games;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;

namespace GeoNRage.App.Pages;

public partial class GamePage : IAsyncDisposable
{
    private HubConnection _hubConnection = null!;

    public GameDetailDto Game { get; set; } = null!;

    [Parameter]
    public int Id { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IGamesApi GamesApi { get; set; } = null!;

    [Inject]
    public IAuthApi AuthApi { get; set; } = null!;

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

    public UserDto User { get; set; } = null!;

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
            User = await AuthApi.CurrentUserInfo();
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
        if (User.PlayerId is null)
        {
            return;
        }

        int clampedValue = Math.Clamp(score ?? 0, 0, 5000);
        await _hubConnection.InvokeAsync("UpdateValue", Id, challengeId, User.PlayerId, round, clampedValue);
        await HandleReceiveValueAsync(challengeId, User.PlayerId, round, clampedValue);
    }

    private async Task AddPlayerAsync()
    {
        if (Game is not null && User.PlayerId is not null && !Game.Players.Any(p => p.Id == User.PlayerId))
        {
            await GamesApi.AddPlayerAsync(Game.Id, User.PlayerId);
            await ReloadPageAsync();
        }
    }
}
