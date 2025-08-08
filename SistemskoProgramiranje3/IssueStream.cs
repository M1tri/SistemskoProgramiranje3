using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class IssueStream : IObservable<GithubIssue>
    {
        private readonly Subject<GithubIssue> issueSubject = new Subject<GithubIssue>();
        private readonly IssueService issueService = new IssueService();

        public async Task GetIssuesAsync()
        {
            try
            {
                var issues = await issueService.GetIssuesAsync();
                foreach (var issue in issues)
                {
                    issueSubject.OnNext(issue);
                }
                issueSubject.OnCompleted();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }
        }

        public IDisposable Subscribe(IObserver<GithubIssue> observer)
        {
            return issueSubject.Subscribe(observer);
        }
    }
}
