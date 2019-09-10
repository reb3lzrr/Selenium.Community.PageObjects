using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.Community.PageObjects.Tests.IntegrationTests.Page;

namespace Selenium.Community.PageObjects.Tests.IntegrationTests
{
    public class IntegrationTests
    {
        [Test]
        public void FindsSingleItem()
        {
            var testPageObject = SetUpFixture.Container.Resolve<TestPageObject>();
            testPageObject.Open();
            testPageObject.PopularSearchesTitle.Text.Should().Contain("Popular searches");
        }

        [Test]
        public void FindsMultipleItems()
        {
            var testPageObject = SetUpFixture.Container.Resolve<TestPageObject>();
            testPageObject.Open();
            testPageObject.PopularSearches.Should().HaveCount(7);
        }

        [Test]
        public void FindsMultipleItems_InOrder()
        {
            var testPageObject = SetUpFixture.Container.Resolve<TestPageObject>();
            testPageObject.Open();
            testPageObject.PopularProducts.Skip(0).First().Name.Text.Should().Be("LG OLED C8");
            testPageObject.PopularProducts.Skip(1).First().Name.Text.Should().Be("Samsung Galaxy S9 Dual Sim");
        }

        [Test]
        public void FindsMultipleItems_Recursive()
        {
            var testPageObject = SetUpFixture.Container.Resolve<TestPageObject>();
            testPageObject.Open();
            testPageObject.PopularProducts.First().Price.Price.Should().Be("€ 1.574,63");
        }

        [Test]
        public void FindsMultipleItems_InRealTime()
        {
            var testPageObject = SetUpFixture.Container.Resolve<TestPageObject>();
            var javaScriptExecutor = SetUpFixture.Container.Resolve<IJavaScriptExecutor>();

            testPageObject.Open();
            testPageObject.PopularProducts.Should().HaveCount(10);
            javaScriptExecutor.ExecuteScript("document.querySelectorAll('#popularProducts div.media:nth-of-type(2n)').forEach(e => e.remove())");
            testPageObject.PopularProducts.Should().HaveCount(6);
        }

        [Test]
        public void FindsMultipleItems_WaitUntil()
        {
            var testPageObject = SetUpFixture.Container.Resolve<TestPageObject>();
            var javaScriptExecutor = SetUpFixture.Container.Resolve<IJavaScriptExecutor>();

            testPageObject.Open();
            testPageObject.PopularProducts.Should().HaveCount(10);

            var removeSomeProductsTask = Task.Run(() =>
            {
                Thread.Sleep(1000);
                javaScriptExecutor.ExecuteScript("document.querySelectorAll('#popularProducts div.media:nth-of-type(2n)').forEach(e => e.remove())");
            });

            var waitForSixProductsTask = Task.Run(() =>
            {
                testPageObject.PopularProducts.WaitUntil(x => x.Count() < 10);
            });

            Task.WaitAll(new [] { removeSomeProductsTask, waitForSixProductsTask}, TimeSpan.FromSeconds(5));
        }
    }
}