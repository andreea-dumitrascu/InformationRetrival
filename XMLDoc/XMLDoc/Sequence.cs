using System.Collections.Generic;

namespace XMLDoc
{
    public class Sequence : Article
    {
        public Sequence(string text, List<string> stopWords, List<string> allWords)
        {
            ParseText(text, wordsDictionary, allWords, stopWords);
        }
    }
}