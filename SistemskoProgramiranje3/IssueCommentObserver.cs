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

        public IssueCommentObserver(string ime)
        {
            this.ime = ime;
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
        }
    }
}
