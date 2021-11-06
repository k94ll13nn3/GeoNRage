namespace GeoNRage.Server.Models;

public record GeoGuessrLogin(string Email, string Password);

public record GeoGuessrProfile(string Id);

public record GeoGuessrChallengeCreator(string Id, string Nick);

public record GeoGuessrChallengeChallenge(int TimeLimit);

public record GeoGuessrChallengeMap(string Id, string Name);

public record GeoGuessrChallengeGame(string Token);

public record GeoGuessrGameGuess(string Token, decimal Lat, decimal Lng, bool TimedOut);

public record GeoGuessrGuess(int RoundScoreInPoints, bool TimedOut, bool TimedOutWithGuess, int Time, double DistanceInMeters);

public record GeoGuessrPin(Uri Url);

public record GeoGuessrPlayer(string Id, string Nick, IList<GeoGuessrGuess> Guesses, GeoGuessrPin Pin);

public record GeoGuessrRound(decimal Lat, decimal Lng);

public record GeoGuessrGame(GeoGuessrPlayer Player, IList<GeoGuessrRound> Rounds);

public record GeoGuessrChallengeResult(GeoGuessrGame Game);

public record GeoGuessrChallenge(
    GeoGuessrChallengeChallenge Challenge,
    GeoGuessrChallengeMap Map,
    GeoGuessrChallengeCreator Creator);
