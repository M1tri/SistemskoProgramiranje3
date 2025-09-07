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

            string[] features1 = new string[] {"error", "crash"};
            string[] features2 = new string[] { "request", "add"};

            double[] outcome = model.Evaluate(features1);
            int index = Array.IndexOf(outcome, outcome.Max());

            string labela = model.GetOutcomeName(index);

            Console.WriteLine(labela);

            outcome = model.Evaluate(features2);
            index = Array.IndexOf(outcome, outcome.Max());

            labela = model.GetOutcomeName(index);

            Console.WriteLine(labela);

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