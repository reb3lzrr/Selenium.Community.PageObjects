using System;
using System.Collections.Generic;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects.Tests.Page;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests
{
    public class FirstClassCitizens
    {
        [Theory]
        [UseContainer]
        public void FindsSingleItem(TestPageObject testPageObject)
        {
            testPageObject.Open();
            testPageObject.PopularSearchesTitle.Text.Should().Contain("Populaire zoekopdrachten");
        }

        [Theory]
        [UseContainer]
        public void FindsMultipleItems(TestPageObject testPageObject)
        {
            testPageObject.Open();
            testPageObject.PopularSearches.Should().HaveCount(7);
        }

        [Theory]
        [UseContainer]
        public void FindsMultipleItems_InRealTime(IJavaScriptExecutor javaScriptExecutor, TestPageObject testPageObject)
        {
            testPageObject.Open();
            testPageObject.MostPopularProducts.Should().HaveCount(10);
            javaScriptExecutor.ExecuteScript("document.querySelectorAll('div.pwPortalPopularProductListing tbody tr:nth-of-type(2n)').forEach(e => e.remove())");
            testPageObject.MostPopularProducts.Should().HaveCount(6);
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
}
