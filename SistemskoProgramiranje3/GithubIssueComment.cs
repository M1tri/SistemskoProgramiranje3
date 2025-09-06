using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class GithubIssueComment
    {
        public long Id { get; set; }
        public required int IssueBroj { get; set; }
        public required string Body { get; set; }    
        public required string Author { get; set; }

        public override string ToString()
        {
            return $"{Id}\n{Author}\n{Body}\n";
        }
    }
}
