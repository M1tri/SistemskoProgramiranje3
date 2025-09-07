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

        public readonly HashSet<string> stopWords;

        public IssueDataCollector(string ime, string tema)
        {
            this.ime = ime;
            this.tema = tema;
            brojevi = new List<int>();
            data = new StringWriter();

            List<string> stopWords = new List<string>(60);
            try
            {
                StreamReader st = new StreamReader("../../stop_words.txt");

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

                st.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            this.stopWords = new HashSet<string>(stopWords, StringComparer.OrdinalIgnoreCase);
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
