using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XMLDoc
{
    public class Article
    {
        Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();
        public List<string> topics = new List<string>();
        public Dictionary<int, int> apparationDictionary = new Dictionary<int, int>();
        public Dictionary<int, double> normalizedDictionary = new Dictionary<int, double>();

        public Article(string path, List<string> stopWords, List<string> allWords)
        {
            XDocument document = XDocument.Load(path);
            GetXmlTitle(document, wordsDictionary, allWords, stopWords);
            GetXmlText(document, wordsDictionary, allWords, stopWords);
            GetXmlTopics(document, topics);
        }
        
        private static void GetXmlTitle(XDocument document, Dictionary<string, int> wordsDictionary, 
            List<string> allWords, List<string> stopWords)
        {
            var title = document.Descendants("title").FirstOrDefault();
            ParseText((string) title, wordsDictionary, allWords, stopWords);
        }

        private static void GetXmlText(XDocument document, Dictionary<string, int> wordsDictionary, 
            List<string> allWords, List<string> stopWords)
        {
            var text = document.Descendants("text").FirstOrDefault();
            ParseText((string)text, wordsDictionary, allWords, stopWords);
        }

        private static void GetXmlTopics(XDocument document, List<string> topics)
        {
            var topicsCodes = document.Descendants("codes").FirstOrDefault()?.NextNode.NextNode;
            var codes = (topicsCodes as XElement)?.DescendantNodes();
            foreach (var code in codes)
            {
                if ((code as XElement)?.FirstAttribute.Name.LocalName == "code")
                {
                    var topic = (code as XElement).FirstAttribute.Value;
                    topics.Add(topic);
                }
            }
        }
        private static void ParseText(string text, Dictionary<string, int> wordsDictionary, 
            List<string> allWords, List<string> stopWords)
        {
            
            MatchCollection matches = Regex.Matches(text.ToLower(), @"\w+|\b(\w\.){2,}|\w+'(\w+){2,}");
            foreach (Match match in matches)
            {
                string word = match.Value;
                PorterStemmer porterStemmer = new PorterStemmer();
                word = porterStemmer.stemTerm(word);
                if (word.Any(char.IsDigit) || word.All(char.IsDigit) || stopWords.Contains(word)) continue;
                if (wordsDictionary.ContainsKey(word))
                {
                    wordsDictionary[word]++;
                }
                else
                {
                    wordsDictionary[word] = 1;
                    if (!allWords.Contains(word))
                    {
                        allWords.Add(word);
                    }
                }
            }

            var orders = wordsDictionary
                .OrderByDescending(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);

            /*foreach (KeyValuePair<string, int> kvp in orders)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }*/
        }
        
        public void MakeApparitionDictionary(List<string> allWords)
        {
            foreach (var word in allWords)
            {
                if (wordsDictionary.ContainsKey(word))
                {
                    apparationDictionary.Add(allWords.IndexOf(word), wordsDictionary[word]);
                }
            }
        }

        //nominal normalization
        public void NormalizeDictionary()
        {
            var max = apparationDictionary.Values.Max();
           
            foreach (var value in apparationDictionary)
            {
                double normalValue;
                normalValue = (1.0 * value.Value) / max;
                normalizedDictionary.Add(value.Key, normalValue);
            }
            
            foreach (KeyValuePair<int, double> kvp in normalizedDictionary)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }

    }
}