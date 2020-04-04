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

            var loginPage = new LoginPage(
                                    int.Parse(config["timeout"]),
                                    config["urlLogin"],
                                    currentLogger);
            loginPage.GoTo();

            var votaPage = loginPage.Logar(config["urlVoto"], config["login"], config["senha"]);
            votaPage.VotarPessoa(config["votarEm"]);
        }
    }
}
