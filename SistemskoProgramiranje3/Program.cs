using System;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SistemskoProgramiranje3
{
    public class Program
    {
        public static async Task Main()
        {
            IssueService issueService = new IssueService();

            var issues = await issueService.GetIssuesAsync();

            foreach (var issue in issues)
            {
                Console.WriteLine(issue);
            }
        }
    }
}