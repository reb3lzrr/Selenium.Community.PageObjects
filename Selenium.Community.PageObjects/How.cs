using OpenQA.Selenium;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// En enum describing the default ways of locating <see cref="IWebElement"/>s. Used in conjuncture with <see cref="FindsByAttribute"/>.
    /// </summary>
    public enum How
    {
        /// <summary>
        /// Finds by <see cref="By.Id" />
        /// </summary>
        Id,

        /// <summary>
        /// Finds by <see cref="By.Name" />
        /// </summary>
        Name,

        /// <summary>
        /// Finds by <see cref="By.TagName" />
        /// </summary>
        TagName,

        /// <summary>
        /// Finds by <see cref="By.ClassName" />
        /// </summary>
        ClassName,

        /// <summary>
        /// Finds by <see cref="By.CssSelector" />
        /// </summary>
        CssSelector,

        /// <summary>
        /// Finds by <see cref="By.LinkText" />
        /// </summary>
        LinkText,

        /// <summary>
        /// Finds by <see cref="By.PartialLinkText" />
        /// </summary>
        PartialLinkText,

        /// <summary>
        /// Finds by <see cref="By.XPath" />
        /// </summary>
        XPath,
    }
}