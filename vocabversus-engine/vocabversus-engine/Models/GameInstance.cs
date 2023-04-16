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

    public class GameInstanceSettings
    {
        /// <summary>
        /// Delay for round to end after end round events
        /// </summary>
        public int RoundEndDelay { get; set; } = 10;
        /// <summary>
        /// Minimum amount of required characters for rounds
        /// </summary>
        public uint MinRequiredChars { get; set; } = 1;
        /// <summary>
        /// Maximum amount of required characters for rounds
        /// </summary>
        public uint MaxRequiredChars { get; set; } = 1;
        /// <summary>
        /// Margin of error for submitted words (e.g. 2 == 2 incorrect characters will still be positively evaluated)
        /// </summary>
        public int IncorrectCharsMargin { get; set; } = 0;
    }

    public class GameInstance
    {
        /// <summary>
        /// Internal ID of game instance
        /// </summary>
        public readonly string Identifier;
        /// <summary>
        /// Gameplay settings
        /// </summary>
        public readonly GameInstanceSettings Settings;
        /// <summary>
        /// Player mananger
        /// </summary>
        public readonly PlayerContainer PlayerInformation;
        /// <summary>
        /// Round manager
        /// </summary>
        public readonly RoundContainer RoundInformation;
        /// <summary>
        /// Current state of game instance
        /// </summary>
        public GameState State { get; set; }

        public GameInstance(string identifier, GameInstanceSettings settings, WordSet wordSet, int maxPlayers)
        {
            Identifier = identifier;
            Settings = settings;
            PlayerInformation = new PlayerContainer(maxPlayers);
            State = GameState.Lobby;
            RoundInformation = new RoundContainer(wordSet, settings.MinRequiredChars, settings.MaxRequiredChars);
        }
    }
}
