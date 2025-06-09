// A utility to analyze text files and provide statistics
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FileAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("File Analyzer - .NET Core");
            Console.WriteLine("This tool analyzes text files and provides statistics.");
            
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a file path as a command-line argument.");
                Console.WriteLine("Example: dotnet run myfile.txt");
                return;
            }
            
            string filePath = args[0];
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File '{filePath}' does not exist.");
                return;
            }
            
            try
            {
                Console.WriteLine($"Analyzing file: {filePath}");
                
                // Read the file content
                string content = File.ReadAllText(filePath);
                
                // Count words
                string[] words = content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                int wordCount = words.Length;
                Console.WriteLine($"Number of words: {wordCount}");
                
                // Count characters (with and without whitespace)
                int charCountWithSpaces = content.Length;
                int charCountWithoutSpaces = content.Count(c => !char.IsWhiteSpace(c));
                Console.WriteLine($"Number of characters (including whitespace): {charCountWithSpaces}");
                Console.WriteLine($"Number of characters (excluding whitespace): {charCountWithoutSpaces}");
                
                // Count sentences
                int sentenceCount = content.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                    .Count(s => s.Trim().Length > 0);
                Console.WriteLine($"Number of sentences: {sentenceCount}");
                
                // Identify most common words
                var wordFrequency = words
                    .GroupBy(w => w.ToLower())
                    .Select(g => new { Word = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .Take(3);
                Console.WriteLine("Top 3 most common words:");
                foreach (var word in wordFrequency)
                {
                    Console.WriteLine($"  {word.Word}: {word.Count} times");
                }
                
                // Average word length
                double averageWordLength = words.Any() ? words.Average(w => w.Length) : 0;
                Console.WriteLine($"Average word length: {averageWordLength:F2} characters");
                
                // Example implementation for counting lines:
                int lineCount = File.ReadAllLines(filePath).Length;
                Console.WriteLine($"Number of lines: {lineCount}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file analysis: {ex.Message}");
            }
        }
    }
}