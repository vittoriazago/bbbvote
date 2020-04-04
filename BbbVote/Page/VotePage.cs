using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Configuration;


namespace BbbVote.Page
{
    internal class VotePage : BaseApplicationPage
    {
        internal readonly string _url;
        public VotePage(int timeout, string url) : base(timeout)
        {
            _url = url;
        }


        internal void GoTo()
        {
            Driver.Navigate().GoToUrl(_url);
        }
    }
}