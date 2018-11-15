using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
          
            var ieOptions = new InternetExplorerOptions { IntroduceInstabilityByIgnoringProtectedModeSettings = true };
            using (var webDriver = new InternetExplorerDriver(".", ieOptions))
            {
                var pageObjectFactory = new PageObjectFactory(webDriver);
                var pageObject = new MasterPageObject(webDriver, pageObjectFactory);

                pageObject.Open();
                pageObject.GridRegels
                    .WaitUntil(x => x.Any())
                    .Skip(10)
                    .First(x => !string.IsNullOrWhiteSpace(x.Artikelen.Text))
                    .WrappedElement.Click();

                pageObject.Artikel.SelectByCode("1450");
            }
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> WaitUntil<T>(this IEnumerable<T> obj, Func<IEnumerable<T>, bool> condition)
        {
            var waiter = new DefaultWait<IEnumerable<T>>(obj);
            waiter.Until(condition);
            return obj;
        }

        public static T WaitUntil<T>(this T obj, Func<T, bool> condition)
        {
            var waiter = new DefaultWait<T>(obj);
            waiter.Until(condition);
            return obj;
        }
    }
}
