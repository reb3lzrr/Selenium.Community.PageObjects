using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace SeleniumExtras.PageObjects.Tests
{
    public class MasterPageObject
    {
        private readonly IWebDriver _webDriver;
        private readonly PageObjectFactory _pageObjectFactory;

        public MasterPageObject(IWebDriver webDriver, PageObjectFactory pageObjectFactory)
        {
            _webDriver = webDriver;
            _pageObjectFactory = pageObjectFactory;
        }

        public void Open()
        {
            _webDriver.Url = "http://bk-webt.bakkerbarendrecht.nl/douane/spa/douane/intern";
            _pageObjectFactory.InitElements(this);
        }


        [FindsBy(How.CssSelector, "button#btn-nieuw")]
        public IWebElement NewButton { get; set; }

        [FindsBy(How.CssSelector, "abap-table-panel[titel='Resultaten'] tbody tr")]
        public IEnumerable<GridRegelPageObject> GridRegels { get; set; }

        [FindsBy(How.CssSelector, "abap-typeahead[name='artikelGroep']")]
        public TypeaheadPageObject Artikelgroep { get; set; }

        [FindsBy(How.CssSelector, "abap-typeahead[name='artikel']")]
        public TypeaheadPageObject Artikel { get; set; }

        public class GridRegelPageObject : IWrapsElement
        {
            public GridRegelPageObject(IWebElement wrappedElement)
            {
                WrappedElement = wrappedElement;
            }

            public IWebElement WrappedElement { get; }

            [FindsBy(How.CssSelector, "td:nth-of-type(1)")]
            public IWebElement Omschrijving { get; set; }

            [FindsBy(How.CssSelector, "td:nth-of-type(2)")]
            public IWebElement GoederenCode { get; set; }

            [FindsBy(How.CssSelector, "td:nth-of-type(3)")]
            public IWebElement Artikelen { get; set; }
        }
    }



    public class TypeaheadPageObject : IWrapsElement
    {
        public IWebElement WrappedElement { get; set; }

        [FindsBy(How.CssSelector, "input[name='omschrijving']")]
        public IWebElement Omschrijving { get; set; }

        [FindsBy(How.CssSelector, "input[name='code']")]
        public IWebElement Code { get; set; }

        public void SelectByCode(string code)
        {
            Code.Clear();
            Code.SendKeys(code);
            Code.SendKeys(Keys.Tab);
        }
        //public void SelectByText(string text)
        //{
        //    if (text == null) return;

        //    var options = SearchGetOptions(text);

        //    //No options...
        //    if (!options.Any())
        //    {
        //        throw new NoSuchElementException($"Option ('{text}') not found");
        //    }

        //    //Too many options
        //    if (options.Count > 1)
        //    {
        //        throw new InvalidOperationException($"There are more then one option available for '{text}'");
        //    }

        //    //There is only one option; click it
        //    options.First().Click();

        //    //Wait until the dropdown thingy dissapeared
        //    new WebDriverWait(.Until(CustomExpectedConditions.ElementNotExists(By.CssSelector("abap-ngb-typeahead-window.dropdown-menu")));
        //}
    }
}
