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
                testPageObject.PopularSearchesTitle.Text.Should().Contain("Populaire zoekopdrachten");
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
                testPageObject.MostPopularProducts.Skip(0).First().Number.Text.Should().Be("1");
                testPageObject.MostPopularProducts.Skip(1).First().Number.Text.Should().Be("2");
            }
        }


        [Fact]
        public void FindsMultipleItems_Recursive()
        {
            using (var container = Container.Build())
            {
                var testPageObject = container.Resolve<TestPageObject>();

                testPageObject.Open();
                testPageObject.MostPopularProducts.First().Price.Anchor.Text.Should().Be("vanaf € 1.574,63");
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
                testPageObject.MostPopularProducts.Should().HaveCount(10);
                javaScriptExecutor.ExecuteScript("document.querySelectorAll('div.pwPortalPopularProductListing tbody tr:nth-of-type(2n)').forEach(e => e.remove())");
                testPageObject.MostPopularProducts.Should().HaveCount(6);
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
            firefoxOptions.AddArgument("--headless");
#if !DEBUG
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
