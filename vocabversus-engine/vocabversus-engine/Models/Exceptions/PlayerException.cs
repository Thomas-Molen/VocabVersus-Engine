namespace vocabversus_engine.Models.Exceptions
{
    [Serializable]
    public class GameInstanceException : Exception
    {
        public GameInstanceException() { }

        public GameInstanceException(string message)
            : base(message) { }

        public GameInstanceException(string message, Exception inner)
            : base(message, inner) { }
    }
}
