using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace Selenium.Community.PageObjects.Tests.IntegrationTests.Page
{
    public class TestPageObject
    {
        private readonly IWebDriver _webDriver;
        private readonly PageObjectFactory _pageObjectFactory;

        public TestPageObject(IWebDriver webDriver, PageObjectFactory pageObjectFactory)
        {
            _webDriver = webDriver;
            _pageObjectFactory = pageObjectFactory;
        }

        public void Open()
        {
            _webDriver.Url = new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "IntegrationTests/Page/test.html")).AbsoluteUri;
            _webDriver.Navigate().Refresh();
            _pageObjectFactory.InitElements(this);
        }

        [FindsBy(How.CssSelector, "#popularProducts div.media")]
        public IEnumerable<ProductPageObject> PopularProducts { get; set; }

        [FindsBy(How.CssSelector, "#popularSearches h2")]
        public IWebElement PopularSearchesTitle { get; set; }

        [FindsBy(How.CssSelector, "#popularSearches > nav > a")]
        public IEnumerable<IWebElement> PopularSearches { get; set; }


        public class ProductPageObject : IWrapsElement
        {
            private readonly IWebDriver _webDriver;

            public ProductPageObject(IWebElement wrappedElement, IWebDriver webDriver)
            {
                _webDriver = webDriver;
                WrappedElement = wrappedElement;
            }

            public IWebElement WrappedElement { get; set; }

            [FindsBy(How.CssSelector, "h5")]
            public IWebElement Name { get; set; }

            [FindsBy(How.CssSelector, "p.price")]
            public PricePageObject Price { get; set; }

            public class PricePageObject : IWrapsElement
            {
                public PricePageObject(IWebElement wrappedElement)
                {
                    WrappedElement = wrappedElement;
                    Price = wrappedElement.Text;
                }

                public IWebElement WrappedElement { get; }

                public string Price { get; }
            }
        }
    }
}