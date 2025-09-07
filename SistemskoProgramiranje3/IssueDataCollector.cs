using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    class IssueDataCollector : IObserver<GithubIssue>
    {
        private readonly string ime;
        private readonly string tema;
        private List<int> brojevi;
        StringWriter data;

        public static readonly HashSet<string> stopWords = new(StringComparer.OrdinalIgnoreCase)
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

        public IssueDataCollector(string ime, string tema)
        {
            this.ime = ime;
            this.tema = tema;
            brojevi = new List<int>();
            data = new StringWriter();
        }

        public void OnNext(GithubIssue issue)
        {
            Console.WriteLine($"{ime}: Obradjujem issue broj {issue.Broj}");

            brojevi.Add(issue.Broj);

            var text = issue.Body.Replace("\n", "")
                                 .Replace("\r", "")
                                 .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                 .Where(r => !stopWords.Contains(r));

            if (text.Count() == 0)
                return;

            data.Write(" ");
            foreach (var word in text)
            {
                data.Write(word.ToLower());
                data.Write(" ");
            }
            data.Write(this.tema);
            data.WriteLine();
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

        public string GetData()
        {
            return data.ToString();
        }
    }
}
