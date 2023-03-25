namespace vocabversus_engine.Utility
{
    // TODO: CHANGE THIS TO AN ACTUAL WORKABLE CACHE
    public class UserReferenceCache
    {
        public Dictionary<string, List<string>> _userGameReferences;

        public UserReferenceCache()
        {
            _userGameReferences = new Dictionary<string, List<string>>();
        }
    }
}
