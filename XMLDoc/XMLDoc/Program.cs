using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace XMLDoc
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            XDocument document = XDocument.Load("/Users/bestiuta/Desktop/Reuters_34/2538NEWS.XML");
            Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();
            List<string> allWords = new List<string>();
            var topics = new List<string>();
            var stopWords = new List<string>();
            InitializeStopWords(stopWords);
            GetXmlTitle(document, wordsDictionary, allWords, stopWords);
            GetXmlText(document, wordsDictionary, allWords, stopWords);
            GetXmlTopics(document, topics);
        }
        
        public static void GetXmlTitle(XDocument document, Dictionary<string, int> wordsDictionary, 
            List<string> allWords, List<string> stopWords)
        {
            var title = document.Descendants("title").FirstOrDefault();
            ParseText((string) title, wordsDictionary, allWords, stopWords);
        }

        public static void GetXmlText(XDocument document, Dictionary<string, int> wordsDictionary, 
            List<string> allWords, List<string> stopWords)
        {
            var text = document.Descendants("text").FirstOrDefault();
            ParseText((string)text, wordsDictionary, allWords, stopWords);
        }

        public static void GetXmlTopics(XDocument document, List<string> topics)
        {
            var topicsCodes = document.Descendants("codes").FirstOrDefault()?.NextNode.NextNode;
            var codes = (topicsCodes as XElement)?.DescendantNodes();
            foreach (var code in codes)
            {
                if ((code as XElement)?.FirstAttribute.Name.LocalName == "code")
                {
                    var topic = (code as XElement).FirstAttribute.Value;
                    topics.Add(topic);
                    Console.WriteLine("topic: " + topic);
                }
            }
        }

        public static void GetAbbr(string text)
        {
            string pattern = @"(?<FirstName>[A-Z]\w*\-?[A-Z]?\w*)\s?(?<MiddleName>[A-Z]\w*|[A-Z]?)\s?(?<LastName>[A-Z]\w*\-?[A-Z]?\w*)(?:,\s|)";
            Regex rg = new Regex(pattern);
            MatchCollection match = rg.Matches(text);
            foreach (Match match1 in match)
            {
                Console.WriteLine(match1.Value);
            }
            string[] separatingStrings = { " ", ",", "'s", ".", "\"", "$", "-", "'re", "'d", "'t", ":", "(", ")", "[", "]"};
            string[] splitWords = text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in splitWords)
            {
                if (word.All(char.IsUpper))
                {
                    Console.WriteLine(word);
                }
            }
        }
        public static void ParseText(string text, Dictionary<string, int> wordsDictionary, 
            List<string> allWords, List<string> stopWords)
        {
            GetAbbr(text);
            string lowerCaseText = text.ToLower();
            string[] separatingStrings = { " ", ",", "'s", ".", "\"", "$", "-", "'re", "'d", "'t", ":"};
            string[] splitWords = lowerCaseText.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in splitWords)
            {
                if (!int.TryParse(word, out _) && !stopWords.Contains(word))
                {
                    if (wordsDictionary.ContainsKey(word))
                    {
                        wordsDictionary[word]++;
                    }
                    else
                    {
                        allWords.Add(word);
                        wordsDictionary[word] = 1;
                    }
                }
            }

            var orders = wordsDictionary
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            
            allWords.Sort();

            foreach (KeyValuePair<string, int> kvp in orders)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }

        public static void InitializeStopWords(List<string> stopWords)
        {
            const string stopWordsPath = "/Users/bestiuta/Documents/Facultate an 3/sem2/regasirea informatiei lab/InformationRetrival/XMLDoc/XMLDoc/Files/stopwords.txt";
            string[] lines = File.ReadAllLines(stopWordsPath);
            foreach (string line in lines)
            {
                stopWords.Add(line);
            }
        }
    }
}