namespace GeoNRage.Shared.Dtos
{
    public class PlayerScoreWithChallengeDto : PlayerScoreDto
    {
        public ChallengeDto Challenge { get; set; } = null!;
    }
}
