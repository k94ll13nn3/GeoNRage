namespace GeoNRage.Server.Models;

internal sealed record GeoGuessrLogin(string Email, string Password);

internal sealed record GeoGuessrProfile(string Id);

internal sealed record GeoGuessrChallengeCreator(string Id, string Nick);

internal sealed record GeoGuessrChallengeChallenge(int TimeLimit);

internal sealed record GeoGuessrChallengeMap(string Id, string Name);

internal sealed record GeoGuessrChallengeGame(string Token);

internal sealed record GeoGuessrGameGuess(string Token, decimal Lat, decimal Lng, bool TimedOut);

internal sealed record GeoGuessrGuess(int RoundScoreInPoints, bool TimedOut, bool TimedOutWithGuess, int Time, double DistanceInMeters, int? StepsCount);

internal sealed record GeoGuessrPin(Uri Url);

internal sealed record GeoGuessrPlayer(string Id, string Nick, IList<GeoGuessrGuess> Guesses, GeoGuessrPin Pin);

internal sealed record GeoGuessrRound(decimal Lat, decimal Lng);

internal sealed record GeoGuessrGame(GeoGuessrPlayer Player, IList<GeoGuessrRound> Rounds);

internal sealed record GeoGuessrChallengeResult(GeoGuessrGame Game);

internal sealed record GeoGuessrChallengeHighscore(IList<GeoGuessrChallengeResult> Items);

internal sealed record GeoGuessrChallenge(
    GeoGuessrChallengeChallenge Challenge,
    GeoGuessrChallengeMap Map,
    GeoGuessrChallengeCreator Creator);

internal sealed record GeoGuessrTitle(string Name);

internal sealed record GeoGuessrLifeTimeXP(GeoGuessrTitle CurrentTitle);

internal sealed record GeoGuessrPlayerStatistics(GeoGuessrLifeTimeXP LifeTimeXpProgression);
