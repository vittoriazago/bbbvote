using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

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

        internal void VotarPessoa(string nomePessoa, int quantidadeVotos)
        {
            for (int voto = 0; voto < quantidadeVotos; voto++)
            {
                VotarPessoa(nomePessoa);
                VotarNovamenteButton.Click();
            }
        }

        internal void VotarPessoa(string nomePessoa)
        {
            _logger.Info($"Votando em {nomePessoa}");

            VerificaElementoCarregado("//div[contains(text(),'Paredão')]");

            var elemento = GetPessoaVoto(nomePessoa);
            elemento.Click();

            ResolveCaptcha();
        }

        private void ResolveCaptcha()
        {
            VerificaElementoClickavel(xpathReload);

            _logger.Info($"Resolvendo captcha");

            var textOfCaptcha = TextImageCaptcha.Text.ToLower();
            var click = ImageUtils.GetImageIndex(Driver, textOfCaptcha, xpathImage);
            if (click == -1)
            {
                VerificaElementoClickavel(xpathReload);
                ReloadCaptchaButton.Click();
                ResolveCaptcha();
            }

            try
            {
                Actions act = new Actions(Driver);
                var xClick = (click * 53) + 25;
                act.MoveToElement(ImageCaptcha).MoveByOffset(xClick, 25).Click().Perform();

                VerificaElementoCarregado(xpathButtonVotarNovamente, 10);
            }
            catch
            {
                VerificaElementoClickavel(xpathReload);
                ReloadCaptchaButton.Click();
                ResolveCaptcha();
            }
        }

        private IWebElement GetPessoaVoto(string nomePessoa)
        {
            return Driver.FindElement(By.XPath($"(//div[div[contains(text(), '{nomePessoa}')]])[2]"));
        }

        public IWebElement TextImageCaptcha => Driver.FindElement(By.XPath("//div[span[contains(text(),'Para confirmar seu voto')]]//span[2]"));

        private readonly string xpathImage = "//div[//span[contains(text(),\\'voto\\')]]//img[contains(@src,\\'base64\\')]";
        public IWebElement ImageCaptcha => Driver.FindElement(By.XPath("//div[//span[contains(text(),'voto')]]//img[contains(@src,'base64')]"));


        private readonly string xpathReload = "//button[contains(@title,'Recarregar')]";
        public IWebElement ReloadCaptchaButton => Driver.FindElement(By.XPath(xpathReload));

        private readonly string xpathButtonVotarNovamente = "//button[contains(text(),'Votar novamente')]";
        public IWebElement VotarNovamenteButton => Driver.FindElement(By.XPath(xpathButtonVotarNovamente));

    }
}