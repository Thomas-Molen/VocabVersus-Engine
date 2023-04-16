namespace vocabversus_engine.Models
{
    public class WordSet
    {
        /// <summary>
        /// Id of word set
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Name of word set
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Words contained within word set
        /// </summary>
        public IEnumerable<string> Words { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Finds a list of random chars that can be used to form words in the word set
        /// </summary>
        /// <param name="charCount">amount of random chars to find</param>
        /// <returns>list of random chars that can form a valid word in word set</returns>
        /// <exception cref="ArgumentException">no list of chars could be created in given word set</exception>
        public char[] GetRandomWordChars(int charCount)
        {
            Random random = new();

            // Find a suitable word to get random chars from
            // This way there will always be a possible word solution for random word chars
            var possibleWords = Words.Where(w => w.Where(c => !char.IsWhiteSpace(c)).Count() > charCount);
            if (!possibleWords.Any()) throw new ArgumentException($"No words exist with the required amount of chars: {charCount}");
            string word = possibleWords.ToArray()[random.Next(possibleWords.Count())];
            char[] wordChars = word.Where(c => !char.IsWhiteSpace(c)).ToArray();

            // get random chars from the chosen word
            char[] resultChars = new char[charCount];
            int resultCharCount = 0;
            while (resultCharCount < charCount)
            {
                // find random char and remove it from available chars as to not create accidental duplicates
                char charToAdd = wordChars[random.Next(wordChars.Length)];
                wordChars = wordChars.Where((v, i) => i != Array.IndexOf(wordChars, charToAdd)).ToArray();

                resultChars[resultCharCount] = charToAdd;
                resultCharCount++;
            }
            return resultChars;
        }
    }
}
