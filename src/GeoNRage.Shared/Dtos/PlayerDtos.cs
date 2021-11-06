using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Players;

public class PlayerEditDto
{
    [Required]
    public string Name { get; set; } = null!;
}

public record PlayerDto(string Id, string Name);

public record PlayerChallengeDto(int ChallengeId, int? GameId, string MapName, int? Sum);

public record PlayerFullStatisticDto(
    int NumberOf5000,
    int NumberOf4999,
    int ChallengesCompleted,
    int? BestGameSum,
    int? BestGameId,
    double? RoundAverage,
    int NumberOf25000,
    double? MapAverage,
    int NumberOf0,
    double? TimeByRoundAverage,
    double? DistanceAverage,
    int NumberOfTimeOut,
    int NumberOfTimeOutWithGuess,
    int? TotalTime,
    double? TotalDistance,
    int? Best5000Time,
    int? Best25000Time);

public record PlayerMapDto(string MapName, int? Best, double? RoundAverage, double? DistanceAverage, double? TimeAverage);

public record PlayerGameDto(int GameId, int Sum, DateTime GameDate);

public record PlayerFullDto(
    string Id,
    string Name,
    Uri? IconUrl,
    IEnumerable<PlayerChallengeDto> ChallengesDone,
    IEnumerable<PlayerChallengeDto> ChallengesNotDone,
    PlayerFullStatisticDto Statistics,
    IEnumerable<PlayerMapDto> MapsSummary,
    IEnumerable<PlayerGameDto> GameHistory);

public record PlayerStatisticDto(
    string Id,
    string Name,
    int NumberOf5000,
    int NumberOf4999,
    int ChallengesCompleted,
    int? BestGameSum,
    int? BestGameId,
    double? RoundAverage);
