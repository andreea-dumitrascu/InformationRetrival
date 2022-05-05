using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace XMLDoc
{
    internal class Program
    {
        static List<string> allWords = new List<string>();
        static List<Article> articles = new List<Article>();
        static List<string> stopWords = new List<string>();
        private static StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "output.txt"));
        private string inputPath;
        private static Sequence input;
        private List<string> articaleOrdersName = new List<string>();
        
        public static void Main(string[] args)
        {
            ArticlesPreparation();
            InputPreparation();
            GetSimilarity();
        }
        
        private static void ArticlesPreparation()
        {
            InitializeStopWords();
            ReadAllArticles();
            allWords.Sort();
            MakeRareMatrix();
            MakeOutputFile();
        }

        private static void InputPreparation()
        {
            string text = File.ReadAllText(@"/Users/bestiuta/Desktop/Interogari de test pentru setul cu 7083 documente.txt");
            input = new Sequence(text, stopWords, allWords);
            input.MakeApparitionDictionary(allWords);
            input.NormalizeDictionary();
        }
        
        private static void InitializeStopWords()
        {
            string stopWordsPath = Directory.GetCurrentDirectory() + "//stopwords.txt";
            string[] lines = File.ReadAllLines(stopWordsPath);
            foreach (string line in lines)
            {
                stopWords.Add(line);
            }
        }

        public static void ReadAllArticles()
        {
            string folderPath = Directory.GetCurrentDirectory() + "//Reuters_34";
            if (Directory.Exists(folderPath))
            {
                foreach (string file in Directory.EnumerateFiles(folderPath, "*.xml"))
                {
                    Article article = new Article(file, stopWords, allWords);
                    articles.Add(article);
                }
            }
            else Console.WriteLine("The folder path does not exists!");
        }

        public static void MakeRareMatrix()
        {
            foreach (var article in articles)
            {
                article.MakeApparitionDictionary(allWords);
            }
        }

        public static void MakeOutputFile()
        {
            using (outputFile)
            {
                foreach (var word in allWords)
                {
                    outputFile.WriteLine("@ATTRIBUTE " + word);
                }
                outputFile.WriteLine();
                outputFile.WriteLine("@DATA");
                foreach (var article in articles)
                {
                    article.NormalizeDictionary();
                    outputFile.WriteLine(article.name);
                    foreach (KeyValuePair<int, int> kvp in article.apparationDictionary)
                    {
                        outputFile.Write("{0}:{1} ", kvp.Key, kvp.Value);
                    }
                    outputFile.Write("# ");
                    foreach (var topic in article.topics)
                    {
                        outputFile.Write(topic + " ");
                    }
                    outputFile.WriteLine();
                    outputFile.WriteLine();
                }
            }
        }

        public static void GetSimilarity()
        {
            foreach (var article in articles)
            {
                var similarity = 0.0;
                for (int i = 0; i < allWords.Count; i++)
                {
                    var articleNormalized = article.normalizedDictionary;
                    var sequenceNormalized = input.normalizedDictionary;
                    var articleItem = 0.0;
                    var sequenceItem = 0.0;
                    
                    if (articleNormalized.ContainsKey(i))
                        articleItem = articleNormalized[i];
                    
                    if(sequenceNormalized.ContainsKey(i))
                        sequenceItem = sequenceNormalized[i];
                    
                    var pow = Math.Pow(articleItem - sequenceItem, 2);
                    similarity = Math.Sqrt(similarity + pow);
                }
                Console.WriteLine(similarity);
            }
        }

        void GetAbbr(string text)
        {
            string pattern =
                @"(?<FirstName>[A-Z]\w*\-?[A-Z]?\w*)\s?(?<MiddleName>[A-Z]\w*|[A-Z]?)\s?(?<LastName>[A-Z]\w*\-?[A-Z]?\w*)(?:,\s|)";
            Regex rg = new Regex(pattern);
            MatchCollection match = rg.Matches(text);
            foreach (Match match1 in match)
            {
                Console.WriteLine(match1.Value);
            }

            string[] separatingStrings =
                {" ", ",", "'s", ".", "\"", "$", "-", "'re", "'d", "'t", ":", "(", ")", "[", "]"};
            string[] splitWords = text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in splitWords)
            {
                if (word.All(char.IsUpper))
                {
                    Console.WriteLine(word);
                }
            }
        }
    }
}