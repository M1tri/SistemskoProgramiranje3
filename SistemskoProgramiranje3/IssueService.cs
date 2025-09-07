using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class IssueService
    {
        private readonly HttpClient httpClient = new HttpClient();

        public string Owner { get; set; }
        public string Repo { get; set; }

        public IssueService(string owner, string repo)
        {
            try
            {
                var token = File.ReadAllText("../../token.txt"); // ovo se ne cuva na repozitorijumu

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Projekat3");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Owner = owner;
            Repo = repo;
        }

        public async Task<IEnumerable<GithubIssue>> GetIssuesAsync()
        {
            var zahtev = $"https://api.github.com/repos/{Owner}/{Repo}/issues";

            var response = await httpClient.GetAsync(zahtev);

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<GithubIssue>();
            }

            var sirov = await response.Content.ReadAsStringAsync();
            var jArray = JArray.Parse(sirov);

            List<GithubIssue> issues = new List<GithubIssue>();

            foreach (var element in jArray)
            {
                GithubIssue issue = new GithubIssue
                {
                    Id = (long)element["id"]!,
                    Broj = (int)element["number"]!,
                    Title = (string)element["title"]!,
                    Body = (string)element["body"]!,
                    User = (string)element["user"]!["login"]!
                };

                issues.Add(issue);
            }

            return issues;
        }
    }
}


