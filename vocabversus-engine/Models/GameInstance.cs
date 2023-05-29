using Microsoft.AspNetCore.Mvc.ModelBinding;
using vocabversus_engine.Utility;

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

    public class Password
    {
        public string HashedPassword { get; set; } = "";
        public byte[] Salt { get; set; } = Array.Empty<byte>();
    }

    public class GameInstance
    {
        /// <summary>
        /// Internal ID of game instance
        /// </summary>
        public readonly string Identifier;
        /// <summary>
        /// Optional password protection
        /// </summary>
        private Password? password = null;
        /// <summary>
        /// If game instance is password protected
        /// </summary>
        public bool IsPasswordProtected => password is not null;
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

        public GameInstance(string identifier, GameInstanceSettings settings, WordSet wordSet, int maxPlayers, string? password = null)
        {
            Identifier = identifier;
            Settings = settings;
            PlayerInformation = new PlayerContainer(maxPlayers);
            State = GameState.Lobby;
            RoundInformation = new RoundContainer(wordSet, settings.MinRequiredChars, settings.MaxRequiredChars);
            
            if (password is not null)
            {
                this.password = new Password
                {
                    HashedPassword = PasswordGenerator.GeneratePassword(password, out byte[] salt),
                    Salt = salt
                };
            }
        }

        public bool VerifyPassword(string password)
        {
            if (!IsPasswordProtected || this.password is null) return true;
            return PasswordGenerator.VerifyPassword(this.password.HashedPassword, this.password.Salt, password);
        }
    }
}
