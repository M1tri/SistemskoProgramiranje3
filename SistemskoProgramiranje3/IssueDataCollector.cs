using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    class IssueDataCollector : IObserver<GithubIssue>
    {
        private readonly string ime;
        private List<int> brojevi;
        StreamWriter dataFile;

        public static readonly HashSet<string> stopWords = new (StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "and", "or", "but", "if", "in", "on", "with",
            "is", "are", "was", "were", "be", "been", "to", "from", "of",
            "for", "by", "at", "this", "that", "these", "those",
            "have", "has", "had", "do", "does", "did",
            "can", "could", "should", "would", "will", "just",
            "i", "you", "he", "she", "it", "we", "they",
            "me", "my", "your", "our", "their",
            "here", "there", "then", "when", "where", "why", "how",
            "issue", "github", "repo", "comment", "thanks", "please", "###"
        };

        public IssueDataCollector(string ime, StreamWriter dataFile)
        {
            this.ime = ime;
            brojevi = new List<int>();
            this.dataFile = dataFile;
        }

        public void OnNext(GithubIssue issue)
        {
            brojevi.Add(issue.Broj);

            var text = issue.Body.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                 .Where(r => !stopWords.Contains(r));

            foreach (var word in text)
            { 
                dataFile.Write(word);
                dataFile.Write(" ");
            }
            dataFile.WriteLine();
        }

        public void OnError(Exception e)
        {
            Console.WriteLine($"{ime}: Doslo je do greske {e.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine($"{ime}: Svi issue su skupljeni");
        }
        public List<int> GetBrojevi()
        {
            return brojevi;
        }
    }
}
