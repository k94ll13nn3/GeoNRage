using System.Security.Claims;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Refit;

namespace GeoNRage.App.Pages.Games;

public partial class GamePage : IAsyncDisposable
{
    private int? _oldGameId;
    private CancellationTokenSource _cancellationToken = new();
    private readonly Dictionary<(int challengeId, string playerId, int round), int?> _scores = [];
    private bool _gameFound = true;
    private bool _loaded;
    private HubConnection _hubConnection = null!;
    private GameDetailDto _game = null!;
    private ClaimsPrincipal _user = null!;

    [Parameter]
    public int Id { get; set; }

    [Parameter]
    [SupplyParameterFromQuery]
    public bool HideScores { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    [Inject]
    public IGamesApi GamesApi { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        await DisposeHubAsync();
        GC.SuppressFinalize(this);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_oldGameId != Id)
        {
            _loaded = false;
            await DisposeHubAsync();
            await ReloadPageAsync();
        }

        _oldGameId = Id;
    }

    protected async Task ReloadPageAsync()
    {
        _loaded = false;
        _user = (await AuthenticationState).User;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/apphub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<int, string, int, int>(
            "ReceiveValue",
            HandleReceiveValue);
        _hubConnection.On(
            "NewPlayerAdded",
            () => ToastService.DisplayToast("Un nouveau joueur a été ajouté à la partie. Veuillez rafraichir la page pour voir ses scores.", null, ToastType.Information, "toast-new-player"));
        _hubConnection.On<string, string>(
            "Taunted",
            (imageId, user) => ToastService.DisplayToast(ImageFragment(TauntImages.Images.GetValueOrDefault(imageId, "img/noob.webp")), null, ToastType.Error, "toast-taunt", title: $"@{user}"));

        _hubConnection.Closed += OnHubConnectionClosed;
        _hubConnection.Reconnecting += OnHubConnectionReconnecting;
        _hubConnection.Reconnected += OnHubConnectionReconnected;

        if (_user.PlayerId() is not null)
        {
            await _hubConnection.StartAsync(_cancellationToken.Token);
        }

        ApiResponse<GameDetailDto> response = await GamesApi.GetAsync(Id);
        if (!response.IsSuccessStatusCode || response.Content is null)
        {
            _gameFound = false;
            return;
        }

        _loaded = true;
        _gameFound = true;
        _game = response.Content;
        foreach (GameChallengeDto challenge in _game.Challenges)
        {
            foreach (string playerId in _game.Players.Select(p => p.Id))
            {
                GameChallengePlayerScoreDto? playerScore = challenge.PlayerScores.FirstOrDefault(p => p.PlayerId == playerId);
                _scores[(challenge.Id, playerId, 1)] = playerScore?.Round1;
                _scores[(challenge.Id, playerId, 2)] = playerScore?.Round2;
                _scores[(challenge.Id, playerId, 3)] = playerScore?.Round3;
                _scores[(challenge.Id, playerId, 4)] = playerScore?.Round4;
                _scores[(challenge.Id, playerId, 5)] = playerScore?.Round5;
            }
        }

        if (_user.PlayerId() is not null)
        {
            await _hubConnection.InvokeAsync("JoinGroup", Id, _cancellationToken.Token);
        }

        StateHasChanged();
    }

    private Task OnHubConnectionReconnected(string? arg)
    {
        ToastService.DisplayToast(
            "Connexion avec le serveur rétablie. Vous pouvez à nouveau envoyer et recevoir des données, cependant les données reçues pendant la tentative de reconnexion ne seront pas mise à jour.",
            null,
            ToastType.Information,
            "signalr-connection",
            true);
        return Task.CompletedTask;
    }

    private Task OnHubConnectionReconnecting(Exception? arg)
    {
        ToastService.DisplayToast(
            "Connexion avec le serveur perdue, tentative de reconnexion...",
            null,
            ToastType.Warning,
            "signalr-connection",
            true);
        return Task.CompletedTask;
    }

    private Task OnHubConnectionClosed(Exception? arg)
    {
        ToastService.DisplayToast(
            "Connexion avec le serveur perdue, reconnexion echouée.",
            null,
            ToastType.Error,
            "signalr-connection",
            true);
        return Task.CompletedTask;
    }

    private void HandleReceiveValue(int challengeId, string playerId, int round, int score)
    {
        _scores[(challengeId, playerId, round)] = score;
        StateHasChanged();
    }

    private async Task SendAsync(int challengeId, int round, int? score)
    {
        if (_user.PlayerId() is not string playerId)
        {
            return;
        }

        int clampedValue = Math.Clamp(score ?? 0, 0, 5000);
        HandleReceiveValue(challengeId, playerId, round, clampedValue);
        if (clampedValue == 5000)
        {
            int numberOfPerfectBefore = _scores
                .Where(p => p.Key.playerId == playerId)
                .TakeWhile(p => !(p.Key.challengeId == challengeId && p.Key.round == round))
                .Reverse()
                .TakeWhile(s => s.Value == 5000)
                .Count();

            string message = numberOfPerfectBefore switch
            {
                0 => "5000 ! Pas mal !",
                < 4 => $"{numberOfPerfectBefore + 1} fois 5000 à la suite ! Quel talent !",
                4 => "5 fois 5000 ! C'est le 25k !",
                _ => "Quel série de 5000 ! O.M.G.",
            };

            ToastService.DisplayToast(message, TimeSpan.FromMilliseconds(2500), ToastType.Success, "toast-5000");
        }

        if (clampedValue == 4999)
        {
            ToastService.DisplayToast(ImageFragment("img/so-close.webp"), TimeSpan.FromMilliseconds(2500), ToastType.Warning, "toast-4999");
        }

        await _hubConnection.InvokeAsync("UpdateValue", Id, challengeId, _user.PlayerId(), round, clampedValue);
    }

    private async Task AddPlayerAsync()
    {
        if (_game is not null && _user.PlayerId() is string playerId && !_game.Players.Any(p => p.Id == _user.PlayerId()))
        {
            await GamesApi.AddPlayerAsync(_game.Id, playerId);
            await _hubConnection.InvokeAsync("NotifyNewPlayer", Id);
            await ReloadPageAsync();
        }
    }

    private async Task TauntAsync((string player, string image) data)
    {
        await _hubConnection.InvokeAsync("TauntPlayer", Id, data.player, data.image);
        ToastService.DisplayToast("Envoyé !", TimeSpan.FromMilliseconds(1500), ToastType.Success, "toast-sent");
    }

    private async Task ShowRankingsAsync()
    {
        await ModalService.DisplayModalAsync<GameRankings>(new()
        {
            [nameof(GameRankings.Scores)] = _scores,
            [nameof(GameRankings.Challenges)] = _game.Challenges,
            [nameof(GameRankings.Players)] = _game.Players,
        },
        ModalOptions.Default);
    }

    private async Task ShowChartAsync()
    {
        await ModalService.DisplayModalAsync<GameChart>(new()
        {
            [nameof(GameChart.Scores)] = _scores,
            [nameof(GameChart.Challenges)] = _game.Challenges,
            [nameof(GameChart.Players)] = _game.Players,
        },
        ModalOptions.Default with { Size = ModalSize.Large });
    }

    private async Task ShowTauntAsync()
    {
        await ModalService.DisplayModalAsync<GameTaunt>(new()
        {
            [nameof(GameTaunt.OnTaunt)] = EventCallback.Factory.Create<(string, string)>(this, TauntAsync),
            [nameof(GameTaunt.Players)] = _game.Players.Where(p => p.Id != _user.PlayerId()),
        },
       ModalOptions.Default);
    }

    private async Task DisposeHubAsync()
    {
        await _cancellationToken.CancelAsync();
        _cancellationToken.Dispose();
        _cancellationToken = new();

        if (_hubConnection is not null)
        {
            _hubConnection.Closed -= OnHubConnectionClosed;
            _hubConnection.Reconnecting -= OnHubConnectionReconnecting;
            _hubConnection.Reconnected -= OnHubConnectionReconnected;
            await _hubConnection.DisposeAsync();
        }
    }
}
