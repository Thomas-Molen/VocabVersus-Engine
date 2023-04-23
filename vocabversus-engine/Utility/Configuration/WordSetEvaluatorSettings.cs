namespace vocabversus_engine.Utility.Configuration
{
    public class WordSetEvaluatorSettings : ISetting
    {
        public const string SectionName = "WordSetEvaluatorSettings";
        public string BaseUrl { get; set; } = string.Empty;
    }
}
