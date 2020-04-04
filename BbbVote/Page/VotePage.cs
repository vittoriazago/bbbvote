using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Configuration;


namespace BbbVote.Page
{
    internal class VotePage : BaseApplicationPage
    {
        internal readonly string _url;
        internal readonly ILogger _logger;
        public VotePage(int timeout, string url, ILogger logger) : base(timeout)
        {
            _url = url;
            _logger = logger;
        }

        internal void GoTo()
        {
            _logger.Info($"Acessando pagina de votação {_url}");
            Driver.Navigate().GoToUrl(_url);
        }

    }
}