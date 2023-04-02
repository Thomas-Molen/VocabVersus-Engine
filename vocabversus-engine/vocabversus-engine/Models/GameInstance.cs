using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace vocabversus_engine.Models
{
    public enum GameState
    {
        Lobby = 0,
        Starting = 1,
        Started = 2,
        Ended = 3,
    }
    public class GameInstance
    {
        public readonly string Identifier;
        public readonly Guid WordSet;
        public readonly PlayerContainer PlayerInformation;
        public GameState State { get; set; }
        public readonly RoundContainer RoundInformation;

        public GameInstance(string identifier, Guid wordSet, int maxPlayers)
        {
            Identifier = identifier;
            WordSet = wordSet;
            PlayerInformation = new PlayerContainer(maxPlayers);
            State = GameState.Lobby;
            RoundInformation = new RoundContainer(wordSet);
        }
    }
}
