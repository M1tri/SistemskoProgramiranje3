using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemskoProgramiranje3
{
    public class GithubIssue
    {
        public long Id { get; set;}
        public required string Title { get; set;}
        public required string Body { get; set;}
        public required string User { get; set;}

        public override string ToString() 
        {
            return $"ID: {Id}\nAutor: {User}\n{Title}\n{Body}";
        }
    }
}
