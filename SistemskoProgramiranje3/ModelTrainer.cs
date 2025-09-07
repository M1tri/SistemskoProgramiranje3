using SharpEntropy;
using SharpEntropy.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.IO;
using File = System.IO.File;

namespace SistemskoProgramiranje3
{
    class ModelTrainer
    {
        private static readonly Dictionary<string, string> Repozitorijumi = new Dictionary<string, string>
        {
              {"AngelAuraMC", "Amethyst-iOS"},
              {"facebook", "react"},
              {"numpy", "numpy"},
              {"Muneerali199", "DocMagic"},
              {"cmintey", "wishlist"},
              {"opensource-society", "notesvault"},
              {"microsoft", "vscode"},
              {"jonbarrow", "trainercards.studio"},
              {"dubinc", "dub"},
              {"vishalmaurya850", "Product-Ledger"},
              {"flarialmc", "dll" }
        };

        private static readonly object lock_obj = new object();

        public static async Task TrenirajModel()
        {
            if (!File.Exists("trainingData.txt"))
                await PrikupiPodatke();

            var trener = new GisTrainer();

            try
            {
                StreamReader stream = new StreamReader("trainingData.txt");
                var dataReader = new PlainTextByLineDataReader(stream);
                BasicEventReader reader = new BasicEventReader(dataReader);

                trener.TrainModel(reader);

                stream.Close();

                PlainTextGisModelWriter modelWriter = new PlainTextGisModelWriter();
                StreamWriter modelStream = new StreamWriter(File.Open("model.txt", FileMode.Create));
                GisModel model = new GisModel(trener);
                modelWriter.Persist(model, modelStream);

                modelStream.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static async Task PrikupiPodatke()
        {
            StreamWriter dataFile;

            try
            {
                dataFile = new StreamWriter(File.Open("trainingData.txt", FileMode.Create));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            List<Task> tasks = new List<Task>();
            foreach (var repo in Repozitorijumi)
            {
                Tuple<string, string, StreamWriter> state = new(repo.Key, repo.Value, dataFile);
                tasks.Add(PrikupiPodatkeNit(state));
            }

            foreach(var t in tasks)
            {
                await t;
            }

            dataFile.Close();
        }

        private static async Task PrikupiPodatkeNit(object? state)
        {
            var (owner,repo,file) = ((Tuple<string, string, StreamWriter>)state!);

            IssueDataCollector bugCollector = new IssueDataCollector("BugCollector", "bug");
            IssueDataCollector featureCollector = new IssueDataCollector("FeatureCollector", "feature");

            IssueStream issueStream = new IssueStream(owner, repo);

            var bugStream = issueStream.Where(i => i.Title.Contains("bug", StringComparison.OrdinalIgnoreCase));
            var featureStream = issueStream.Where(i => i.Title.Contains("feature", StringComparison.OrdinalIgnoreCase) ||
                                                       i.Title.Contains("enhancement", StringComparison.OrdinalIgnoreCase) ||
                                                       i.Title.Contains("improvement", StringComparison.OrdinalIgnoreCase));
            bugStream.Subscribe(bugCollector);
            featureStream.Subscribe(featureCollector);

            var issues = await issueStream.GetIssuesAsync();

            IssueCommentStream issueCommentStream = new IssueCommentStream(owner, repo);

            var featureCommentStream = issueCommentStream.Where(c => featureCollector.GetBrojevi().Contains(c.IssueBroj));
            var bugCommentStream = issueCommentStream.Where(c => bugCollector.GetBrojevi().Contains(c.IssueBroj));

            CommentDataCollector bugCommentCollector = new CommentDataCollector("BugCommentCollector", "bug");
            CommentDataCollector featureCommentCollector = new CommentDataCollector("FeatureCommentCollector", "feature");

            featureCommentStream.Subscribe(featureCommentCollector);
            bugCommentStream.Subscribe(bugCommentCollector);

            await issueCommentStream.GetCommentsAsync(issues);

            try
            {
                lock (lock_obj)
                {
                    file.Write(bugCollector.GetData());
                    file.Write(featureCollector.GetData());

                    file.Write(bugCommentCollector.GetData());
                    file.Write(featureCommentCollector.GetData());
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
