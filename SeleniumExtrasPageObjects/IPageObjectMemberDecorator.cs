using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// Interface describing how members of a class which represent elements in a Page Object
    /// are detected.
    /// </summary>
    public interface IPageObjectMemberDecorator
    {
        /// <summary>
        /// Locates an element or list of elements for a Page Object member.
        /// </summary>
        /// <param name="typeToDecorate">The <see cref="Type"/> of the member to decorate</param>
        /// <param name="bys">The <see cref="By"> bys</see> provided to decorate the member with</param>
        /// <param name="elementLocator">The <see cref="IElementLocator"/> elementLocator to locate elements.</param>
        /// <returns>The Page Object's member value</returns>
        object Decorate(Type typeToDecorate, IEnumerable<By> bys, IElementLocator elementLocator);
    }
}