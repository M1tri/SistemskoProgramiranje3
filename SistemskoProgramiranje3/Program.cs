using System;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpEntropy;
using SharpEntropy.IO;

namespace SistemskoProgramiranje3
{
    public class Program
    {
        public static async Task Main()
        {
            if (!File.Exists("model.txt"))
                await ModelTrainer.TrenirajModel();

            StreamReader modelStream = new StreamReader("model.txt");
            PlainTextGisModelReader modelReader = new PlainTextGisModelReader(modelStream);

            GisModel model = new GisModel(modelReader);

            var commentStream = new IssueCommentStream("M1tri", "SistemskoProgramiranje3");

            var observerDime = new IssueCommentObserver("Gledam dime", model);
            var observerDunja = new IssueCommentObserver("Gledam dunja", model);

            var filtriranoDime = commentStream.Where(c => c.Author == "M1tri");
            var filtriranoDunja = commentStream.Where(c => c.Author == "dunjajovic");

            var subscriber1 = filtriranoDime.Subscribe(observerDime);
            var subscriber2 = filtriranoDunja.Subscribe(observerDunja);

            await commentStream.GetCommentsAsync();

            Console.ReadLine();
        }
    }
}