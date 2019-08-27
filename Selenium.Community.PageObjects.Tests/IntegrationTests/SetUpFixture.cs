using System;
using Autofac;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
// ReSharper disable once RedundantUsingDirective
using OpenQA.Selenium.Firefox;
using Selenium.Community.PageObjects.Tests.IntegrationTests.Page;

namespace Selenium.Community.PageObjects.Tests.IntegrationTests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        public static IContainer Container { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Container = BuildContainer();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Container.Dispose();
        }

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            IWebDriver webDriver = null;

#if !DEBUG
            var firefoxOptions = new FirefoxOptions();
            firefoxOptions.AddArgument("--headless");
            var driverService = FirefoxDriverService.CreateDefaultService();
            driverService.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            driverService.HideCommandPromptWindow = true;
            driverService.SuppressInitialDiagnosticInformation = true;
            webDriver = new FirefoxDriver(driverService, firefoxOptions, TimeSpan.FromSeconds(60));
#endif
#if DEBUG
            webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory));
#endif
            
            builder.RegisterInstance(webDriver)
                .As<IWebDriver>()
                .As<IJavaScriptExecutor>()
                .SingleInstance()
                .OnRelease(firefoxDriver =>
                {
                    firefoxDriver.Quit();
                    firefoxDriver.Dispose();
                });
            builder.RegisterType<PageObjectFactory>();
            builder.RegisterType<TestPageObject>();

            return builder.Build();
        }
    }
}
