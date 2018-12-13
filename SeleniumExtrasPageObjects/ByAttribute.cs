using OpenQA.Selenium;
using System;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// An attribute that serves as the root for inheritence.
    /// The <see cref="PageObjectFactory"></see> will search for <see cref="ByAttribute"/> and load the Element Accordingly
    /// </summary>
    public abstract class ByAttribute : Attribute
    {
        /// <summary>
        /// Gets an explicit <see cref="By"/> object to find by.
        /// </summary>
        public abstract By ByFinder();
    }
}