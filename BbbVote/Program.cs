using BbbVote.Page;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
            votaPage.VotarPessoa(config["votarEm"], 10);
        }
    }
}
