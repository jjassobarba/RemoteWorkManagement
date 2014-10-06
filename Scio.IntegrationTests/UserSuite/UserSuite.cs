using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Scio.IntegrationTests.UserSuite
{
    [TestFixture]
    public class UserSuite
    {
        /// <summary>
        /// The driver
        /// </summary>
        private IWebDriver _driver;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        [Test]
        public void CreateUser()
        {
            _driver.Navigate().GoToUrl("http://localhost:49560/Account/Login");
            _driver.Manage().Window.Maximize();
            var random = new Random();
            var userNameField = _driver.FindElement(By.Id("username"));
            var passwordField = _driver.FindElement(By.Id("password"));
            var loginButton = _driver.FindElement(By.Name("LoginButton"));
            userNameField.SendKeys("admin@rewoma.com");
            passwordField.SendKeys("upM9kE3N"); //We should change this line for the correct password
            loginButton.Click();
            var usersLink = _driver.FindElement(By.CssSelector("a[href*='Users']"));
            usersLink.Click();
            var randomData = "test" + random.Next(0, 999)*11111;
            var firstName = _driver.FindElement(By.Name("firstname"));
            firstName.SendKeys(randomData);
            var lastName = _driver.FindElement(By.Name("lastname"));
            lastName.SendKeys("lastName" + randomData);
            var email = _driver.FindElement(By.Name("username"));
            email.SendKeys(randomData + "@gmail.com");
            var position = _driver.FindElement(By.Name("position"));
            position.SendKeys("app dev");
            var role = _driver.FindElement(By.XPath("//select[@id='role-select']/option[@value='0']"));
            role.Click();
        }
    }
}
