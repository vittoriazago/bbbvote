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
            var outPutDirectory = GetAssemblysOutputDirectory();
            var directoryWithChromeDriver = CreateFilePathForNetCoreApps(outPutDirectory);
            if (string.IsNullOrEmpty(directoryWithChromeDriver))
            {
                directoryWithChromeDriver = CreateFilePathForNetFrameworkApps(outPutDirectory);
            }
            return new ChromeDriver(directoryWithChromeDriver);
        }

        private static string CreateFilePathForNetFrameworkApps(string outPutDirectory)
        {
            //If the outputDirectory is null, a new exception will be thrown
            //Otherwise, we will concatenate the path and create the correct one
            return Path.GetFullPath(Path.Combine(
                                outPutDirectory ?? throw new InvalidOperationException(),
                                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/"));
        }

        private static string CreateFilePathForNetCoreApps(string outPutDirectory)
        {
            var resourcesDirectory = "";
            if (outPutDirectory != null && outPutDirectory.Contains("netcoreapp"))
                resourcesDirectory = Path.GetFullPath(Path.Combine(outPutDirectory, 
                                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/"));
            return resourcesDirectory;
        }

        private static string GetAssemblysOutputDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private string GetSeleniumBinaryLocation()
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.GetFullPath(Path.Combine(outPutDirectory, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/"));
        }
    }
}
