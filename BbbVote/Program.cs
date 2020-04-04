using BbbVote.Page;
using Microsoft.Extensions.Configuration;
using NLog;
using System;

namespace BbbVote
{
    class Program
    {
        static readonly IConfiguration config = Configuration.InitConfiguration();

        static void Main(string[] args)
        {
            var currentLogger = LogManager.GetCurrentClassLogger();

            var votaPage = new VotePage(
                                    int.Parse(config["timeout"]), 
                                    config["url"],
                                    currentLogger);
            votaPage.GoTo();
        }
    }
}
