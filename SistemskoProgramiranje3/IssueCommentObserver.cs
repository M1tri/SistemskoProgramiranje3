using SharpEntropy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class IssueCommentObserver : IObserver<GithubIssueComment>
    {
        private readonly string ime;
        GisModel model;
        public readonly HashSet<string> stopWords;

        public IssueCommentObserver(string ime, GisModel model)
        {
            this.ime = ime;
            this.model = model;

            StreamReader st = new StreamReader("../../stop_words.txt");
            List<string> stopWords = new List<string>();

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
            var features = value.Body.Replace("\n", " ")
                                      .Replace("\n", " ")
                                      .Split(' ')
                                      .Where(r => !stopWords.Contains(r))
                                      .ToArray();

            double[] verovatnoce = model.Evaluate(features);
            int maxIndex = Array.IndexOf(verovatnoce, verovatnoce.Max());

            string tema = model.GetOutcomeName(maxIndex);

            Console.WriteLine($"Ja sam {ime}"); 
            Console.WriteLine($"i stiglo mi je {value}");
            Console.WriteLine($"Tema ovog komentara je {tema}");
        }
    }
}
