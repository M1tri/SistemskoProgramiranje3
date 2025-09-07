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

        public readonly HashSet<string> stopWords;

        public CommentDataCollector(string ime, string tema)
        {
            this.ime = ime;
            data = new StringWriter();
            this.tema = tema;

            StreamReader st = new StreamReader("../../stop_words.txt");
            List<string> stopWords = new List<string>(50);

            while (!st.EndOfStream)
            {
                var line = st.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var word in words)
                        stopWords.Add(word);
                }
            }

            this.stopWords = new HashSet<string>(stopWords, StringComparer.OrdinalIgnoreCase);
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
