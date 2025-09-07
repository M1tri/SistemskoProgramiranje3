using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class IssueCommentStream : IObservable<GithubIssueComment>
    {
        private readonly Subject<GithubIssueComment> commentSubject = new Subject<GithubIssueComment>();
        private readonly IssueCommentService issueCommentService;
        private readonly IssueService issueService;

        private TaskCompletionSource<bool> prikupljeno = new TaskCompletionSource<bool>();

        public string Owner {
            get => issueCommentService.Owner;
            set { 
                issueCommentService.Owner = value;
                issueService.Owner = value;
            }
        }

        public string Repo
        {
            get => issueCommentService.Repo;
            set
            {
                issueCommentService.Repo = value;
                issueService.Repo = value;
            }
        }

        public IssueCommentStream(string owner, string repo)
        {
            this.issueCommentService = new IssueCommentService(owner, repo);
            this.issueService = new IssueService(owner, repo);
        }

        public async Task GetCommentsAsync(IEnumerable<GithubIssue>? issues = null)
        {
            try
            {
                if (issues == null)
                {
                    issues = await issueService.GetIssuesAsync();
                }

                // Za svaki issue pribavljamo komentari na background niti
                issues
                    .ToObservable()
                    .SelectMany(issue =>
                        Observable.FromAsync(() => issueCommentService.GetCommentsAsync(issue.Broj))
                                  .SubscribeOn(TaskPoolScheduler.Default)
                                  .SelectMany(comments => comments)
                    )
                    .Subscribe(
                        comment => {
                            Console.WriteLine($"Iz niti: (Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}): {comment.IssueBroj}");
                            commentSubject.OnNext(comment);
                            },
                        ex => commentSubject.OnError(ex),
                        () => {
                            commentSubject.OnCompleted();
                            prikupljeno.SetResult(true);
                        });

                await prikupljeno.Task;

                /*
                foreach (var issue in issues)
                {
                    var comments = await issueCommentService.GetCommentsAsync(issue.Broj); // ovo u niti nekada TODO

                    foreach (var comment in comments)
                    {
                        commentSubject.OnNext(comment);
                    }
                    commentSubject.OnCompleted();
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public IDisposable Subscribe(IObserver<GithubIssueComment> observer)
        {
            return commentSubject.Subscribe(observer);
        }
    }
}
