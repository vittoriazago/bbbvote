using NLog;
using OpenQA.Selenium;

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

        public VotePage(IWebDriver driver, ILogger logger, int timeout) : base(timeout, driver)
        {
            Driver = driver;
            _logger = logger;
        }

        internal void GoTo()
        {
            _logger.Info($"Acessando pagina de votação {_url}");
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

        internal void VotarPessoa(string nomePessoa)
        {
            _logger.Info($"Votando em {nomePessoa}");


            VerificaElementoCarregado("//div[contains(text(),'Paredão')]");

            var elemento = GetPessoaVoto(nomePessoa);
            elemento.Click();

            VerificaElementoClickavel(xpathReload);

            var textOfCaptcha = TextImageCaptcha.Text.ToLower();
            var click = ImageUtils.GetImage(Driver, textOfCaptcha, xpathImage);
        }

        private IWebElement GetPessoaVoto(string nomePessoa)
        {
            return Driver.FindElement(By.XPath($"/html/body/div[2]/div[4]/div/div[1]/div[4]/div[2]/div"));
        }

        public IWebElement TextImageCaptcha => Driver.FindElement(By.XPath("//div[//span[contains(text(),'voto')]]//span[2]"));

        private string xpathImage = "//div[//span[contains(text(),'voto')]]//img";
        public IWebElement ImageOfCaptcha => Driver.FindElement(By.XPath(xpathImage));

        private string xpathReload = "//button[contains(text(),'Recarregar')]";
        public IWebElement ReloadCaptchaButton => Driver.FindElement(By.XPath(xpathReload));

    }
}