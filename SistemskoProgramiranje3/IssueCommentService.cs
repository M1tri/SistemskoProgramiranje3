using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class IssueCommentService
    {
        private readonly HttpClient httpClient = new HttpClient();
        public IssueCommentService() 
        {
            try
            {
                var token = File.ReadAllText("token.txt"); // ovo se ne cuva na repozitorijumu

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Projekat3");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<IEnumerable<GithubIssueComment>> GetCommentsAsync(int brojIssue)
        {
            var owner = "M1tri";
            var repo = "SistemskoProgramiranje3";

            var zahtev = $"https://api.github.com/repos/{owner}/{repo}/issues/{brojIssue}/comments";

            var respone = await httpClient.GetAsync(zahtev);

            if (!respone.IsSuccessStatusCode)
            {
                return Enumerable.Empty<GithubIssueComment>();
            }

            var sirov = await respone.Content.ReadAsStringAsync();
            var json = JArray.Parse(sirov);

            List<GithubIssueComment> comments = new List<GithubIssueComment>(); 
            
            foreach (var item in json)
            {
                GithubIssueComment comment = new GithubIssueComment
                {
                    Id = (long)item["id"],
                    Body = (string)item["body"],
                    Author = (string)item["user"]["login"],
                };

                comments.Add(comment);  
            }

            return comments;
        }
    }
}
