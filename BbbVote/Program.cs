using BbbVote.Page;
using Microsoft.Extensions.Configuration;
using System;

namespace BbbVote
{
    class Program
    {
        static readonly IConfiguration config = Configuration.InitConfiguration();

        static void Main(string[] args)
        {

            var votaPage = new VotePage(int.Parse(config["timeout"]), config["url"]);
        }
    }
}
