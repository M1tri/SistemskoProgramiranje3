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
            await ModelTrainer.PrikupiPodatke();

            return;

            var commentStream = new IssueCommentStream("M1tri", "SistemskoProgramiranje3");

            var observerDime = new IssueCommentObserver("Gledam dime");
            var observerDunja = new IssueCommentObserver("Gledam dunja");

            var filtriranoDime = commentStream.Where(c => c.Author == "M1tri");
            var filtriranoDunja = commentStream.Where(c => c.Author == "dunjajovic");

            var subscriber1 = filtriranoDime.Subscribe(observerDime);
            var subscriber2 = filtriranoDunja.Subscribe(observerDunja);

            await commentStream.GetCommentsAsync();

            Console.ReadLine();
        }
    }
}