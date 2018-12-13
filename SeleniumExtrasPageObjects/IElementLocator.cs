using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// Interface describing how elements are to be located by a <see cref="PageObjectFactory"/>.
    /// </summary>
    public interface IElementLocator
    {
        /// <summary>
        /// Locates an element using the given list of <see cref="By"/> criteria.
        /// </summary>
        /// <param name="bys">The list of methods by which to search for the element.</param>
        /// <returns>An <see cref="IWebElement"/> which is the first match under the desired criteria.</returns>
        IWebElement LocateElement(IEnumerable<By> bys);

        /// <summary>
        /// Locates a collection of elements using the given list of <see cref="By"/> criteria.
        /// </summary>
        /// <param name="bys">The different methods by which to search for the elements.</param>
        /// <returns>A collection of all elements which match the desired criteria.</returns>
        ReadOnlyCollection<IWebElement> LocateElements(IEnumerable<By> bys);
    }
}