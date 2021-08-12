using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Challenges;

public class ChallengeImportDto
{
    [Required]
    public string GeoGuessrId { get; set; } = null!;

    [Required]
    public bool OverrideData { get; set; }
}

public record ChallengeAdminViewDto(int Id, string MapId, string MapName, string GeoGuessrId, int GameId, string GameName, DateTime LastUpdate, bool UpToDate);

public record ChallengeDetailDto(int Id, string MapName, string GeoGuessrId, IEnumerable<ChallengePlayerScoreDto> PlayerScores);

public record ChallengePlayerScoreDto(string PlayerId, string PlayerName, IEnumerable<ChallengePlayerGuessDto> PlayerGuesses);

public record ChallengePlayerGuessDto(int RoundNumber, int? Score, int? Time, double? Distance);

public record ChallengeDto(int Id, string MapId, string MapName, string GeoGuessrId, int? GameId, string? CreatorName, int MaxScore);
