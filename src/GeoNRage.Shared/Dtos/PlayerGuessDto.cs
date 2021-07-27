namespace GeoNRage.Shared.Dtos
{
    public class PlayerGuessDto
    {
        public int RoundNumber { get; set; }

        public int? Score { get; set; }

        public bool TimedOut { get; set; }

        public bool TimedOutWithGuess { get; set; }

        public int? Time { get; set; }

        public double? Distance { get; set; }
    }
}
