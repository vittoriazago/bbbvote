using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace BbbVote.Browser
{
    public class WebDriverFactory
    {
        public IWebDriver Create(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return GetChromeDriver();
                default:
                    throw new ArgumentOutOfRangeException("No such browser exists");
            }
        }
        private IWebDriver GetChromeDriver()
        {
            var chromeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
           
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.PageLoadStrategy = PageLoadStrategy.None;

            var driver = new ChromeDriver(chromeDirectory, options, TimeSpan.FromSeconds(30));
            return driver;
        }

    }
}
