using System;
using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// Provides the ability to produce Page Objects modeling a page. This class cannot be inherited.
    /// </summary>
    public class PageObjectFactory
    {
        private readonly IElementLocator _elementLocator;
        private readonly IElementActivator _elementActivator;
        private readonly DefaultWait<IWebDriver> _webDriverWait;

        private const BindingFlags PublicBindingOptions = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags NonPublicBindingOptions = BindingFlags.Instance | BindingFlags.NonPublic;

        public PageObjectFactory(IWebDriver driver) :
            this(new DefaultElementActivator(), 
                new DefaultElementLocator(driver), 
                new WebDriverWait(driver, TimeSpan.FromSeconds(10)))
        {

        }

        public PageObjectFactory(IElementActivator elementActivator, IElementLocator elementLocator, DefaultWait<IWebDriver> webDriverWait)
        {
            _elementActivator = elementActivator ?? throw new ArgumentException("Argument can not be null", nameof(elementActivator));
            _elementLocator = elementLocator ?? throw new ArgumentException("Argument can not be null", nameof(elementLocator));
            _webDriverWait = webDriverWait;
        }

        /// <summary>
        /// Populates the members decorated with the <see cref="FindsByAttribute"/>
        /// of the <param name="page">pageObject</param>.
        /// </summary>
        /// <param name="page">The pageObject</param>
        public void InitElements(object page)
        {
            InitElements(page, _elementLocator, new ProxyPageObjectMemberDecorator(_elementActivator, this, _webDriverWait));
        }

        internal void InitElements(object page, IElementLocator locator, IPageObjectMemberDecorator decorator)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page), "page cannot be null");
            }

            if (locator == null)
            {
                throw new ArgumentNullException(nameof(locator), "locator cannot be null");
            }

            if (decorator == null)
            {
                throw new ArgumentNullException(nameof(locator), "decorator cannot be null");
            }

            if (locator.SearchContext == null)
            {
                throw new ArgumentException("The SearchContext of the locator object cannot be null", nameof(locator));
            }

            // Get a list of all of the fields and properties (public and non-public [private, protected, etc.])
            // in the passed-in page object. Note that we walk the inheritance tree to get superclass members.
            var type = page.GetType();
            var members = new List<MemberInfo>();
            members.AddRange(type.GetFields(PublicBindingOptions));
            members.AddRange(type.GetProperties(PublicBindingOptions));
            while (type != null)
            {
                members.AddRange(type.GetFields(NonPublicBindingOptions));
                members.AddRange(type.GetProperties(NonPublicBindingOptions));
                type = type.BaseType;
            }

            foreach (var member in members)
            {
                //Decorates the member
                var decoratedValue = decorator.Decorate(member, locator);
                if (decoratedValue == null)
                {
                    continue;
                }

                var field = member as FieldInfo;
                var property = member as PropertyInfo;
                if (field != null)
                {
                    field.SetValue(page, decoratedValue);
                }
                else if (property != null)
                {
                    property.SetValue(page, decoratedValue, null);
                }
            }
        }
    }
}
