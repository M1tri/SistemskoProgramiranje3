using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

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
              {"microsoft", "vscode"}
        };

        public static async Task PrikupiPodatke()
        {
            StreamWriter bugFile;
            StreamWriter featureFile;

            try
            {
                bugFile = new StreamWriter(File.Open("bugs.txt", FileMode.Create));
                featureFile = new StreamWriter(File.Open("features.txt", FileMode.Create));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            foreach (var repo in Repozitorijumi)
            {
                IssueDataCollector bugCollector = new IssueDataCollector("BugCollector", bugFile);
                IssueDataCollector featureCollector = new IssueDataCollector("FeatureCollector", featureFile);

                IssueStream issueStream = new IssueStream(repo.Key, repo.Value);

                var bugStream = issueStream.Where(i => i.Title.Contains("bug", StringComparison.OrdinalIgnoreCase));
                var featureStream = issueStream.Where(i => i.Title.Contains("feature", StringComparison.OrdinalIgnoreCase) ||
                                                           i.Title.Contains("enhancement", StringComparison.OrdinalIgnoreCase));

                bugStream.Subscribe(bugCollector);
                featureStream.Subscribe(featureCollector);

                var issues = await issueStream.GetIssuesAsync();

                IssueCommentStream issueCommentStream = new IssueCommentStream(repo.Key, repo.Value);

                var featureCommentStream = issueCommentStream.Where(c => featureCollector.GetBrojevi().Contains(c.IssueBroj));
                var bugCommentStream = issueCommentStream.Where(c => bugCollector.GetBrojevi().Contains(c.IssueBroj));

                CommentDataCollector bugCommentCollector = new CommentDataCollector("BugCommentCollector");
                CommentDataCollector featureCommentCollector = new CommentDataCollector("FeatureCommentCollector");

                featureCommentStream.Subscribe(featureCommentCollector);
                bugCommentStream.Subscribe(bugCommentCollector);

                await issueCommentStream.GetCommentsAsync(issues);

                bugFile.WriteLine(bugCommentCollector.GetData());
                featureFile.WriteLine(featureCommentCollector.GetData());
            }

            bugFile.Close();
            featureFile.Close();
        }
    }
}
