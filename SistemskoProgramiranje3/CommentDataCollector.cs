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
        private readonly string tema;

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

        public CommentDataCollector(string ime, string tema)
        {
            this.ime = ime;
            data = new StringWriter();
            this.tema = tema;
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
            Console.WriteLine($"Ja sam {ime} i obradjujem {value.IssueBroj} {value.Id}");

            var text = value.Body.Replace("\n", "")
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
        public string GetData()
        {
            return data.ToString();
        }
    }
}
