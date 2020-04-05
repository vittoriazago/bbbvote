using BbbVote.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace BbbVote.Page
{
    public class BaseApplicationPage : IDisposable
    {
        internal readonly int _timeout;

        public BaseApplicationPage(int timeout)
        {
            _timeout = timeout;

            var factory = new WebDriverFactory();
            Driver = factory.Create(BrowserType.Firefox);
            //Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(100);
        }

        public BaseApplicationPage(int timeout, IWebDriver driver)
        {
            Driver = driver;
            _timeout = timeout;
        }

        protected IWebDriver Driver { get; set; }

        internal bool VerificaUrlCarregada(string url)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(_timeout));
            return wait.Until(ExpectedConditions.UrlContains(url));
        }

        internal bool VerificaElementoCarregado(string xpath, int? timeout = null)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout ?? _timeout));
            return wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath))) != null;
        }

        internal bool VerificaElementoClickavel(string xpath)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(_timeout));
            return wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpath))) != null;
        }

        public void Dispose()
        {
            if (Driver == null)
                return;
            Driver.Quit();
            Driver = null;
        }
    }
}