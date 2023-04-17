namespace vocabversus_engine.Hubs.GameHub
{
    public enum GameHubExceptionCode
    {
        UnkownError = 0,
        IdentifierNotFound = 100,
        UserNotFound = 200,
        UserAddFailed = 201,
        UserEditFailed = 202,
        ActionNotAllowed = 300,
    }
}
