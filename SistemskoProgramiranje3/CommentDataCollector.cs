using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    class CommentDataCollector : IObserver<GithubIssueComment>
    {
        private readonly string ime;

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

        public CommentDataCollector(string ime)
        {
            this.ime = ime;
            data = new StringWriter();
        }

        public void OnCompleted()
        {
            Console.WriteLine($"Ja sam {ime} i sve je stiglo uspesno");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"Ja sam {ime} i doslo je do greske!");
        }

        public void OnNext(GithubIssueComment value)
        {
            Console.WriteLine($"Ja sam {ime} i stiglo mi je {value}");

            var text = value.Body.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                     .Where(r => !stopWords.Contains(r));

            foreach (var word in text)
            {
                data.Write(word);
                data.Write(" ");
            }
            data.WriteLine();
        }
        public string GetData()
        {
            return data.ToString();
        }
    }
}
