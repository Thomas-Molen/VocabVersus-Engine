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
        public MaximumPlayerException() { }

        public MaximumPlayerException(string message)
            : base(message) { }

        public MaximumPlayerException(string message, Exception inner)
            : base(message, inner) { }
    }

    [Serializable]
    public class DuplicatePlayerException : PlayerException
    {
        public DuplicatePlayerException() { }

        public DuplicatePlayerException(string message)
            : base(message) { }

        public DuplicatePlayerException(string message, Exception inner)
            : base(message, inner) { }
    }
}
