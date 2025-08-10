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
        private readonly IssueCommentService issueCommentService = new IssueCommentService();
        private readonly IssueService issueService = new IssueService();

        public async Task GetCommentsAsync()
        {
            try
            {
                var issues = await issueService.GetIssuesAsync();

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
                            Console.WriteLine($"Iz niti: (Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}): {comment.Body}");
                            commentSubject.OnNext(comment);
                            },
                        ex => commentSubject.OnError(ex),
                        () => commentSubject.OnCompleted());

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
