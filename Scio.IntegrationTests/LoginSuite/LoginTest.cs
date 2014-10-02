using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Scio.IntegrationTests.LoginSuite
{
    [TestFixture]
    public class LoginTest
    {

        /// <summary>
        /// The _driver
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
        /// Logins the admin.
        /// </summary>
        [Test]
        public void LoginAdmin()
        {
            _driver.Navigate().GoToUrl("http://localhost:49560/Account/Login");
            _driver.Manage().Window.Maximize();
            var userNameField = _driver.FindElement(By.Id("username"));
            var passwordField = _driver.FindElement(By.Id("password"));
            var loginButton = _driver.FindElement(By.Name("LoginButton"));
            userNameField.SendKeys("admin@rewoma.com");
            passwordField.SendKeys("fakePass"); //We should change this line for the correct password
            loginButton.Click();
            var result = _driver.FindElement(By.Id("myStats")).Text;
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Fails the login.
        /// </summary>
        [Test]
        public void FailLogin()
        {
            _driver.Navigate().GoToUrl("http://localhost:49560/Account/Login");
            _driver.Manage().Window.Maximize();
            var userNameField = _driver.FindElement(By.Id("username"));
            var passwordField = _driver.FindElement(By.Id("password"));
            var loginButton = _driver.FindElement(By.Name("LoginButton"));
            userNameField.SendKeys("admin@rewoma.com");
            passwordField.SendKeys("fakePass");
            loginButton.Click();
            var result = _driver.FindElement(By.ClassName("validation-summary-errors")).Text;
            Assert.That(result, Is.EqualTo("Invalid usernarme or password"));
        }

    }
}
