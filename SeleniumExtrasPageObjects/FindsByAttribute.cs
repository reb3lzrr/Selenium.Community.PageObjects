using System;
using System.Globalization;
using OpenQA.Selenium;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// Marks program elements with methods by which to find a corresponding element on the page. Used
    /// in conjunction with the <see cref="PageObjectFactory"/>, it allows you to quickly create Page Objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can use this attribute by specifying the <see cref="_how"/> and <see cref="_uising"/> properties
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
        private readonly string _uising;

        /// <summary>
        /// Creates a new instance of the FindsByAttribute, allowing to 
        /// </summary>
        /// <param name="how"></param>
        /// <param name="using"></param>
        public FindsByAttribute(How how, string @using)
        {
            _how = how;
            _uising = @using;
        }

        /// <inheritdoc cref="ByAttribute.ByFinder"/>
        public override By ByFinder()
        {
            return _finder = _finder ?? From(this);
        }

        public By From(FindsByAttribute attribute)
        {
            switch (_how)
            {
                case How.Id:
                    return By.Id(_uising);
                case How.Name:
                    return By.Name(_uising);
                case How.TagName:
                    return By.TagName(_uising);
                case How.ClassName:
                    return By.ClassName(_uising);
                case How.CssSelector:
                    return By.CssSelector(_uising);
                case How.LinkText:
                    return By.LinkText(_uising);
                case How.PartialLinkText:
                    return By.PartialLinkText(_uising);
                case How.XPath:
                    return By.XPath(_uising);
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Did not know how to construct How from how {0}, using {1}", _how, _uising));
            }
        }


       
        /// <summary>
        /// Determines whether the specified <see cref="object">Object</see> is equal
        /// to the current <see cref="object">Object</see>.
        /// </summary>
        /// <param name="obj">The <see cref="object">Object</see> to compare with the
        /// current <see cref="object">Object</see>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="object">Object</see>
        /// is equal to the current <see cref="object">Object</see>; otherwise,
        /// <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as FindsByAttribute;
            if (other == null)
            {
                return false;
            }

            if (other.ByFinder() != ByFinder())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="object">Object</see>.</returns>
        public override int GetHashCode()
        {
            return ByFinder().GetHashCode();
        }
    }
}