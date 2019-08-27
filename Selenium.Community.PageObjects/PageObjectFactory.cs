﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// Provides the ability to produce Page Objects modeling a page
    /// </summary>
    public class PageObjectFactory
    {
        private readonly IElementLocator _elementLocator;
        private readonly IPageObjectMemberDecorator _pageObjectMemberDecorator;

        private const BindingFlags PublicBindingOptions = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags NonPublicBindingOptions = BindingFlags.Instance | BindingFlags.NonPublic;

        public PageObjectFactory(IWebDriver webDriver)
        {
            _elementLocator = new DefaultElementLocator(webDriver);
            _pageObjectMemberDecorator = new ProxyPageObjectMemberDecorator(new DefaultElementActivator(), this, webDriver);
        }

        public PageObjectFactory(IElementLocator elementLocator, IPageObjectMemberDecorator pageObjectMemberDecorator)
        {
            _elementLocator = elementLocator ?? throw new ArgumentException("Argument can not be null", nameof(elementLocator));
            _pageObjectMemberDecorator = pageObjectMemberDecorator;
        }

        /// <summary>
        /// Populates the members decorated with the <see cref="FindsByAttribute"/>
        /// of the <param name="page">pageObject</param>.
        /// </summary>
        /// <param name="page">The pageObject</param>
        public void InitElements(object page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page), "page cannot be null");
            }

            InitElements(page, _elementLocator);
        }

        internal void InitElements(object page, IElementLocator locator)
        {
            foreach (var member in MembersToDecorate(page))
            {
                var bys = member.GetCustomAttributes()
                    .Select(x => (x as ByAttribute)?.ByFinder())
                    .Where(x => x != null)
                    .ToArray();

                if (bys.Any())
                {
                    //Check if member can be written to
                    if (!CanWriteToMember(member, out var typeToDecorate))
                    {
                        throw new MemberAccessException($"Can't write to {member.DeclaringType.Name}.{member.Name} whilst decorated with a {nameof(ByAttribute)}");
                    }

                    //Decorates the member
                    var decoratedValue = _pageObjectMemberDecorator.Decorate(typeToDecorate, bys, locator);
                    if (decoratedValue != null)
                    {
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

        private static List<MemberInfo> MembersToDecorate(object page)
        {
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

            return members;
        }

        private static bool CanWriteToMember(MemberInfo member, out Type type)
        {
            if (member is FieldInfo field)
            {
                type = field.FieldType;
                return true;
            }

            if (member is PropertyInfo property)
            {
                type = property.PropertyType;
                return property.CanWrite;
            }

            type = null;
            return false;
        }
    }
}