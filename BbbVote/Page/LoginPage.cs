using NLog;
using OpenQA.Selenium;
using System;

namespace BbbVote.Page
{
    internal class LoginPage : BaseApplicationPage
    {
        internal readonly string _url;
        internal readonly ILogger _logger;

        public LoginPage(int timeout, string url, ILogger logger) : base(timeout)
        {
            _url = url;
            _logger = logger;
        }

        internal void GoTo()
        {
            _logger.Info($"Acessando pagina de login {_url}");
            Driver.Navigate().GoToUrl(_url);
        }

        public bool IsLoaded
        {
            get
            {
                var isLoaded = VerificaUrlCarregada("login");
                _logger.Trace($"Página carregada=>{isLoaded}");
                return isLoaded;

            }
        }

        internal VotePage Logar(string urlRedirect, string login, string password)
        {
            _logger.Info($"Logando com {login}");
            VerificaElementoCarregado("//a[contains(text(), 'Esqueceu')]");

            Login.SendKeys(login);
            Senha.SendKeys(password);

            BotaoEntrar.Click();

            VerificaElementoCarregado("//div[@class='myAccountPicture']");

            Driver.Navigate().GoToUrl(urlRedirect);
            return new VotePage(Driver, _logger, _timeout);
        }

        public IWebElement Login => Driver.FindElement(By.XPath("//input[@id='login']"));
        public IWebElement Senha => Driver.FindElement(By.XPath("//input[@id='password']"));
        public IWebElement BotaoEntrar => Driver.FindElement(By.XPath("//button[contains(text(),'Entrar')]"));
   
    }
}