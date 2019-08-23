using System;
using System.Linq;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Selenium.Community.PageObjects;
using Selenium.Community.PageObjects.Tests.IntegrationTests;
using Selenium.Community.PageObjects.Tests.IntegrationTests.Page;

namespace Selenium.Community.PageObjects.Tests.IntegrationTests
{
    public class IntegrationTests
    {
        [Theory]
        [IntegrationTest]
        public void FindsSingleItem(TestPageObject testPageObject)
        {
            testPageObject.Open();
            testPageObject.PopularSearchesTitle.Text.Should().Contain("Popular searches");
        }

        [Theory]
        [IntegrationTest]
        public void FindsMultipleItems(TestPageObject testPageObject)
        {

            testPageObject.Open();
            testPageObject.PopularSearches.Should().HaveCount(7);
        }

        [Theory]
        [IntegrationTest]
        public void FindsMultipleItems_InOrder(TestPageObject testPageObject)
        {
            testPageObject.Open();
            testPageObject.PopularProducts.Skip(0).First().Name.Text.Should().Be("LG OLED C8");
            testPageObject.PopularProducts.Skip(1).First().Name.Text.Should().Be("Samsung Galaxy S9 Dual Sim");
        }

        [Theory]
        [IntegrationTest]
        public void FindsMultipleItems_Recursive(TestPageObject testPageObject)
        {

            testPageObject.Open();
            testPageObject.PopularProducts.First().Price.Price.Should().Be("€ 1.574,63");
        }

        [Theory]
        [IntegrationTest]
        public void FindsMultipleItems_InRealTime(TestPageObject testPageObject, IJavaScriptExecutor javaScriptExecutor)
        {
            testPageObject.Open();
            testPageObject.PopularProducts.Should().HaveCount(10);
            javaScriptExecutor.ExecuteScript(
                "document.querySelectorAll('#popularProducts div.media:nth-of-type(2n)').forEach(e => e.remove())");
            testPageObject.PopularProducts.Should().HaveCount(6);
        }
    }
}

class TearDown
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        IntegrationTestAttribute.Container = BuildContainer();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        IntegrationTestAttribute.Container.Dispose();
    }

    public static IContainer BuildContainer()
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

        builder.Register(c => new FirefoxDriver(driverService, firefoxOptions, TimeSpan.FromSeconds(60)))
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
