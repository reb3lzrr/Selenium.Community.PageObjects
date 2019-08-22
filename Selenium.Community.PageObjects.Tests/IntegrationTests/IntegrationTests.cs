using System;
using System.Linq;
using Autofac;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Selenium.Community.PageObjects.Tests.IntegrationTests.Page;
using Xunit;

namespace Selenium.Community.PageObjects.Tests.IntegrationTests
{
    public class IntegrationTests
    {
        [Fact]
        public void FindsSingleItem()
        {
            using (var container = Container.Build())
            {
                var testPageObject = container.Resolve<TestPageObject>();

                testPageObject.Open();
                testPageObject.PopularSearchesTitle.Text.Should().Contain("Popular searches");
            }
        }

        [Fact]
        public void FindsMultipleItems()
        {
            using (var container = Container.Build())
            {
                var testPageObject = container.Resolve<TestPageObject>();

                testPageObject.Open();
                testPageObject.PopularSearches.Should().HaveCount(7);
            }
        }

        [Fact]
        public void FindsMultipleItems_InOrder()
        {
            using (var container = Container.Build())
            {
                var testPageObject = container.Resolve<TestPageObject>();

                testPageObject.Open();
                testPageObject.PopularProducts.Skip(0).First().Name.Text.Should().Be("LG OLED C8");
                testPageObject.PopularProducts.Skip(1).First().Name.Text.Should().Be("Samsung Galaxy S9 Dual Sim");
            }
        }


        [Fact]
        public void FindsMultipleItems_Recursive()
        {
            using (var container = Container.Build())
            {
                var testPageObject = container.Resolve<TestPageObject>();

                testPageObject.Open();
                testPageObject.PopularProducts.First().Price.Price.Should().Be("€ 1.574,63");
            }
        }

        [Fact]
        public void FindsMultipleItems_InRealTime()
        {
            using (var container = Container.Build())
            {
                var testPageObject = container.Resolve<TestPageObject>();
                var javaScriptExecutor = container.Resolve<IJavaScriptExecutor>();

                testPageObject.Open();
                testPageObject.PopularProducts.Should().HaveCount(10);
                javaScriptExecutor.ExecuteScript("document.querySelectorAll('#popularProducts div.media:nth-of-type(2n)').forEach(e => e.remove())");
                testPageObject.PopularProducts.Should().HaveCount(6);
            }
        }
    }

    public static class Container
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            FirefoxDriverService driverService = null;
            var firefoxOptions = new FirefoxOptions();

#if !DEBUG
            firefoxOptions.AddArgument("--headless");
            driverService = FirefoxDriverService.CreateDefaultService();
            driverService.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            driverService.HideCommandPromptWindow = true;
            driverService.SuppressInitialDiagnosticInformation = true;
#endif
#if DEBUG
            driverService = FirefoxDriverService.CreateDefaultService(Environment.CurrentDirectory);
#endif

            builder.Register(c => new FirefoxDriver(driverService, firefoxOptions , TimeSpan.FromSeconds(60)))
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
