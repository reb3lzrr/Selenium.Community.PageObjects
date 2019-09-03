using System;
using System.Globalization;
using OpenQA.Selenium;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// Marks program elements with methods by which to find a corresponding element on the page. Used
    /// in conjunction with the <see cref="PageObjectFactory"/>, it allows you to quickly create Page Objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can use this attribute by specifying the <see cref="_how"/> and <see cref="_using"/> properties
    /// to indicate how to find the elements. This attribute can be used to decorate fields and properties
    /// in your Page Object classes. The <see cref="Type"/> of the field or property must be either
    /// <see cref="IWebElement"/> or IList{IWebElement}. Any other type will throw an
    /// <see cref="ArgumentException"/> when <see cref="PageObjectFactory.InitElements(object)"/> is called.
    /// </para>
    /// <para>
    /// <code>
    /// [FindsBy(How = How.Name, Using = "myElementName")]
    /// public IWebElement foundElement;
    ///
    /// [FindsBy(How = How.TagName, Using = "a")]
    /// public IList{IWebElement} allLinks;
    /// </code>
    /// </para>
    /// <para>
    /// You can also use multiple instances of this attribute to find an element that may meet
    /// one of multiple criteria.
    /// </para>
    /// <para>
    /// <code>
    /// // Will find the element with the name attribute matching the first of "anElementName"
    /// // or "differentElementName".
    /// [FindsBy(How = How.Name, Using = "anElementName")]
    /// [FindsBy(How = How.Name, Using = "differentElementName")]
    /// public IWebElement thisElement;
    /// </code>
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FindsByAttribute : ByAttribute
    {
        private By _finder;
        private readonly How _how;
        private readonly string _using;

        /// <summary>
        /// Creates a new instance of the FindsByAttribute, allowing to 
        /// </summary>
        /// <param name="how"></param>
        /// <param name="using"></param>
        public FindsByAttribute(How how, string @using)
        {
            _how = how;
            _using = @using;
        }

        /// <inheritdoc cref="ByAttribute.ByFinder"/>
        public override By ByFinder()
        {
            return _finder = _finder ?? GetFinder();
        }

        private By GetFinder()
        {
            switch (_how)
            {
                case How.Id:
                    return By.Id(_using);
                case How.Name:
                    return By.Name(_using);
                case How.TagName:
                    return By.TagName(_using);
                case How.ClassName:
                    return By.ClassName(_using);
                case How.CssSelector:
                    return By.CssSelector(_using);
                case How.LinkText:
                    return By.LinkText(_using);
                case How.PartialLinkText:
                    return By.PartialLinkText(_using);
                case How.XPath:
                    return By.XPath(_using);
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Did not know how to construct How from how {0}, using {1}", _how, _using));
            }
        }
    }
}