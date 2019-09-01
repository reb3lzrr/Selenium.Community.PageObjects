using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// A default locator for elements for use with the <see cref="PageObjectFactory"/>. This locator
    /// implements no retry logic for elements not being found, nor for elements being stale.
    /// </summary>
    public class DefaultElementLocator : IElementLocator
    {
        private readonly IWait<ISearchContext> _waiter;
        private readonly ISearchContext _searchContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementLocator"/> class.
        /// </summary>
        /// <param name="searchContext">The <see cref="ISearchContext"/> used by this locator
        /// to locate elements.</param>
        public DefaultElementLocator(ISearchContext searchContext)
        {
            _searchContext = searchContext ?? throw new ArgumentNullException(nameof(searchContext));
            _waiter = new DefaultWait<ISearchContext>(searchContext)
            {
                PollingInterval = TimeSpan.FromMilliseconds(5),
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        /// <summary>
        /// Locates an element using the given list of <see cref="By"/> criteria.
        /// </summary>
        /// <param name="bys">The list of methods by which to search for the element.</param>
        /// <returns>An <see cref="IWebElement"/> which is the first match under the desired criteria.</returns>
        public IWebElement LocateElement(IEnumerable<By> bys)
        {
            if (!(bys?.Any() ?? false))
            {
                throw new ArgumentNullException(nameof(bys), "List of criteria may not be null or empty");
            }

            try
            {
                return _waiter.Until(searchContext => FindElements(bys.Distinct(), searchContext).FirstOrDefault());
            }
            catch (WebDriverTimeoutException)
            {
                throw new NoSuchElementException($"Could not find any elements matching the provided bys: {string.Join(", ", bys)}");
            }
        }

        /// <summary>
        /// Locates a list of elements using the given list of <see cref="By"/> criteria.
        /// </summary>
        /// <param name="bys">The list of methods by which to search for the elements.</param>
        /// <returns>A list of all elements which match the desired criteria.</returns>
        public IReadOnlyCollection<IWebElement> LocateElements(IEnumerable<By> bys)
        {
            if (!(bys?.Any() ?? false))
            {
                throw new ArgumentNullException(nameof(bys), "List of criteria may not be null or empty");
            }

            return FindElements(bys.Distinct(), _searchContext).ToArray();
        }

        private IEnumerable<IWebElement> FindElements(IEnumerable<By> bys, ISearchContext searchContext)
        {
            return bys.SelectMany(by =>
            {
                try
                {
                    return searchContext.FindElements(by).AsEnumerable();
                }
                catch (NoSuchElementException)
                {
                    return new IWebElement[0];
                }
            });
        }
    }
}
