using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GeoNRage.Shared.Dtos.Players
{
    public class PlayerEditDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }

    public record PlayerDto(string Id, string Name);

    public record PlayerGuessDto(int RoundNumber, int? Score, bool TimedOut, bool TimedOutWithGuess, int? Time, double? Distance);

    public record PlayerScoreWithChallengeDto(
        int? Sum,
        bool Done,
        IEnumerable<PlayerGuessDto> PlayerGuesses,
        int ChallengeId,
        int? ChallengeTimeLimit,
        string MapId,
        string MapName,
        int? GameId,
        DateTime GameDate,
        bool MapIsMapForGame);

    public record PlayerFullDto(string Id, string Name, IEnumerable<PlayerScoreWithChallengeDto> PlayerScores);

    public record PlayerStatisticDto(
        string Id,
        string Name,
        int NumberOf5000,
        int NumberOf4999,
        int ChallengesCompleted,
        int? BestGameSum,
        int? BestGameId,
        int RoundAverage);
}
