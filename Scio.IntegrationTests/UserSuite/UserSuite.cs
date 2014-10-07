using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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
            var roleChosen = _driver.FindElement(By.Id("role_select_chosen"));
            roleChosen.Click();
            var selectRole = _driver.FindElement(By.ClassName("active-result"));
            selectRole.Click();
            var leaderChosen = _driver.FindElement(By.Id("plSelect_chosen"));
            leaderChosen.Click();
            var leaderChosenResults = _driver.FindElement(By.ClassName("active-result"));
            leaderChosenResults.Click();
            var senseiChosen = _driver.FindElement(By.Id("senseiSelect_chosen"));
            senseiChosen.Click();
            var workRemoteDay = _driver.FindElement(By.Id("Monday"));
            workRemoteDay.Click();
            var flexTime = _driver.FindElement(By.Id("08:00 - 17:00"));
            flexTime.Click();
            var registerButton = _driver.FindElement(By.ClassName("progress-button"));
            registerButton.Click();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            userNameField = wait.Until(driver => driver.FindElement(By.Name("firstname")));
            Assert.IsEmpty(userNameField.Text);
        }
    }
}
