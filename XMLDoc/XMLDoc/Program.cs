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

        public static void Main(string[] args)
        {
            InitializeStopWords();
            ReadAllArticles();
            allWords.Sort();
            MakeDictionaryIndexesFile();
            MakeRareMatrix();

            
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
                Console.WriteLine("------------------------------");
            }
        }

        public static void MakeDictionaryIndexesFile()
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "dictionary_indexs.txt")))
            {
                foreach (string word in allWords)
                {
                    outputFile.WriteLine(word + " " + allWords.IndexOf(word));
                    
                }
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