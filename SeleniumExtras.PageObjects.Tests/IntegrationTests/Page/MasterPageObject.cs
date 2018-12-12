﻿using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace SeleniumExtras.PageObjects.Tests.Page
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


        [FindsBy(How.CssSelector, "div.pwPortalPopularProductListing tbody tr")]
        public IEnumerable<ProductPageObject> MostPopularProducts { get; set; }

        [FindsBy(How.CssSelector, "div.greyTopBorderBlock.popularsearches")]
        public IWebElement PopularSearchesTitle { get; set; }

        [FindsBy(How.CssSelector, "div.greyTopBorderBlock.popularsearches a")]
        public IEnumerable<IWebElement>  PopularSearches { get; set; }

        public class ProductPageObject : IWrapsElement
        {
            public ProductPageObject(IWebElement wrappedElement)
            {
                WrappedElement = wrappedElement;
            }

            public IWebElement WrappedElement { get; set; }

            [FindsBy(How.CssSelector, "span.counter")]
            public IWebElement Number { get; set; }

            [FindsBy(How.CssSelector, "p.price")]
            public IWebElement Price { get; set; }
        }
    }
}