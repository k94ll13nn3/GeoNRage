using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Games;

public class GameChallengeCreateOrEditDto
{
    public int Id { get; set; }

    [Required]
    public string MapId { get; set; } = null!;

    [Required]
    public string GeoGuessrId { get; set; } = null!;
}

public class GameCreateOrEditDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    [Required]
    public ICollection<string> PlayerIds { get; set; } = [];

    [Required]
    public ICollection<GameChallengeCreateOrEditDto> Challenges { get; set; } = [];
}

public record GameDto(int Id, string Name, DateTime Date);

public record GameChallengeInfoDto(int Id, string MapId, string GeoGuessrId);

public record GameAdminViewDto(int Id, string Name, DateTime Date, IEnumerable<GameChallengeInfoDto> Challenges, IEnumerable<string> PlayerIds);

public record GameChallengePlayerScoreDto(string PlayerId, string PlayerName, int? Round1, int? Round2, int? Round3, int? Round4, int? Round5, int? Sum);

public record GameChallengeDto(int Id, string MapId, string GeoGuessrId, string MapName, IEnumerable<GameChallengePlayerScoreDto> PlayerScores);

public record GamePlayerDto(string Id, string Name);

public record GameDetailDto(int Id, string Name, DateTime Date, IEnumerable<GameChallengeDto> Challenges, IEnumerable<GamePlayerDto> Players);
