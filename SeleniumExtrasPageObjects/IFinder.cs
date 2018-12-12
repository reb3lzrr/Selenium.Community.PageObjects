using OpenQA.Selenium;

namespace SeleniumExtras.PageObjects
{
    public interface IFinder
    {
        /// <summary>
        /// Gets an explicit <see cref="By"/> object to find by.
        /// </summary>
        By Finder();
    }
}