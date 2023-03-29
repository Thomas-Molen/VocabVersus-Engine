namespace vocabversus_engine.Models.Exceptions
{
    [Serializable]
    public class PlayerException : Exception
    {
        public PlayerException() { }

        public PlayerException(string message)
            : base(message) { }

        public PlayerException(string message, Exception inner)
            : base(message, inner) { }
    }

    [Serializable]
    public class MaximumPlayerException : PlayerException
    {
        public MaximumPlayerException(string message)
            : base(message) { }
    }

    [Serializable]
    public class DuplicatePlayerException : PlayerException
    {
        public DuplicatePlayerException(string message)
            : base(message) { }
    }

    [Serializable]
    public class MissingPlayerException : PlayerException
    {
        public MissingPlayerException(string message)
            : base(message) { }
    }
}
