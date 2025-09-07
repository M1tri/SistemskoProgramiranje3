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
        private readonly IssueService issueService;
        public IssueStream(string owner, string repo)
        {
            issueService = new IssueService(owner, repo);
        }

        public async Task<IEnumerable<GithubIssue>> GetIssuesAsync()
        {
            try
            {
                var issues = await issueService.GetIssuesAsync();
                foreach (var issue in issues)
                {
                    issueSubject.OnNext(issue);
                }
                issueSubject.OnCompleted();

                return issues;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return Enumerable.Empty<GithubIssue>();
            }
        }

        public IDisposable Subscribe(IObserver<GithubIssue> observer)
        {
            return issueSubject.Subscribe(observer);
        }
    }
}
