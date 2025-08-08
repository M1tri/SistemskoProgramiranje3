using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class IssueObserver : IObserver<GithubIssue>
    {
        private readonly string ime;

        public IssueObserver(string ime)
        {
            this.ime = ime;
        }

        public void OnNext(GithubIssue issue)
        {
            Console.WriteLine($"Ja sam {ime} i stiglo mi je\n{issue}\n");
        }

        public void OnError(Exception e)
        {
            Console.WriteLine($"{ime}: Doslo je do greske {e.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine($"{ime}: Svi issue su skupljeni");
        }
    }
}