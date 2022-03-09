using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XMLDoc
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            XDocument document = XDocument.Load("/Users/bestiuta/Desktop/Reuters_34/2538NEWS.XML");
            var topics = new List<string>();
            Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();
            List<string> allWords = new List<string>();
            GetXmlTitle(document, wordsDictionary, allWords);
            GetXmlText(document, wordsDictionary, allWords);
            GetXmlTopics(document, topics);
        }

        public static void GetXmlTitle(XDocument document, Dictionary<string, int> wordsDictionary, List<string> allWords)
        {
            var title = document.Descendants("title").FirstOrDefault();
            ParseText((string) title, wordsDictionary, allWords);
        }

        public static void GetXmlText(XDocument document, Dictionary<string, int> wordsDictionary, List<string> allWords)
        {
            var text = document.Descendants("text").FirstOrDefault();
            ParseText((string)text, wordsDictionary, allWords);
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

        public static void ParseText(string text, Dictionary<string, int> wordsDictionary, List<string> allWords)
        {
            string lowerCaseText = ((string) text).ToLower();
            string[] separatingStrings = { " ", ",", "'s", ".", "\"", "$", "-", "'re", "'d", "'t"};
            string[] splitWords = lowerCaseText.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in splitWords)
            {
                if (!int.TryParse(word, out _))
                {
                    if (wordsDictionary.ContainsKey(word))
                    {
                        wordsDictionary[word]++;
                        allWords.Add(word);
                    }
                    else
                    {
                        wordsDictionary[word] = 1;
                    }
                }
            }

            var orders = wordsDictionary.OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
            
            foreach (KeyValuePair<string, int> kvp in orders)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }
    }
}