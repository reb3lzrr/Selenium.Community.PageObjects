using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects.Tests.Page;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests
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

    public static class Extensions
    {
        public static IEnumerable<T> WaitUntil<T>(this IEnumerable<T> obj, Func<IEnumerable<T>, bool> condition)
        {
            var waiter = new DefaultWait<IEnumerable<T>>(obj)
            {
                PollingInterval = TimeSpan.FromMilliseconds(50),
                Timeout = TimeSpan.FromSeconds(10)
            };
            waiter.Until(condition);
            return obj;
        }
    }

    public static class Container
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EdgeDriver>()
                .WithParameter("service", EdgeDriverService.CreateDefaultService("."))
                .As<IWebDriver>()
                .As<IJavaScriptExecutor>()
                .SingleInstance();
            builder.RegisterType<PageObjectFactory>();
            builder.RegisterType<TestPageObject>();

            return builder.Build();
        }
    }
}
