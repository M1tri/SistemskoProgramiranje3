using System;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SistemskoProgramiranje3
{
    public class Program
    {
        public static async Task Main()
        {
            var issueStream = new IssueStream();

            var observerDime = new IssueObserver("Gledam dime");
            var observerDunja = new IssueObserver("Gledam dunju");

            var filtriranoDime = issueStream.Where(i => i.User == "M1tri");
            var filtriranoDunja = issueStream.Where(i => i.User == "dunjajovic");

            var subscriber1 = filtriranoDime.Subscribe(observerDime);
            var subscriber2 = filtriranoDunja.Subscribe(observerDunja);

            await issueStream.GetIssuesAsync();

            Console.ReadLine();
        }
    }
}