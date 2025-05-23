using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Challenges;

public class ChallengeImportDto
{
    [Required]
    [MinLength(16)]
    [MaxLength(17)]
    public string GeoGuessrId { get; set; } = null!;

    [Required]
    public bool OverrideData { get; set; }
}

public record ChallengeAdminViewDto(int Id, string MapId, string MapName, string GeoGuessrId, int GameId, string GameName, DateTime? LastUpdate, bool UpToDate, int CompletedBy);

public record ChallengeDetailDto(int Id, string MapName, string GeoGuessrId, IEnumerable<ChallengePlayerScoreDto> PlayerScores);

public record ChallengePlayerScoreDto(string PlayerId, string PlayerName, IEnumerable<ChallengePlayerGuessDto> PlayerGuesses);

public record ChallengePlayerGuessDto(int RoundNumber, int? Score, int? Time, double? Distance);

public record ChallengeDto
{
    public required int Id { get; init; }
    public required string MapId { get; init; }
    public required string MapName { get; init; }
    public required string GeoGuessrId { get; init; }
    public required string? CreatorName { get; init; }
    public required int MaxScore { get; init; }
    public required int? PlayerScore { get; init; }
    public required DateOnly? CreatedAt { get; init; }
}
