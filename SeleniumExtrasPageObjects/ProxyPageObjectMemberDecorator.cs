using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects.Proxies;

namespace SeleniumExtras.PageObjects
{
    public class ProxyPageObjectMemberDecorator : IPageObjectMemberDecorator
    {
        private readonly IElementActivator _elementActivator;
        private readonly PageObjectFactory _factory;
        private readonly DefaultWait<IWebDriver> _webDriverWaiter;

        public ProxyPageObjectMemberDecorator(IElementActivator elementActivator, PageObjectFactory factory, DefaultWait<IWebDriver> webDriverWaiter)
        {
            _elementActivator = elementActivator;
            _factory = factory;
            _webDriverWaiter = webDriverWaiter;
        }

        public object Decorate(MemberInfo member, IElementLocator locator)
        {
            var findsByAttributes = member.GetCustomAttributes<FindsByAttribute>().ToArray();

            if (findsByAttributes.Any() && TryCanDecorate(member, out var typeToDecorate))
            {
                var bys = findsByAttributes.Select(ByFactory.From);

                if (typeof(IWebElement).IsAssignableFrom(typeToDecorate))
                {
                    return DecorateWebElement(locator, bys);
                }

                if (typeof(IWrapsElement).IsAssignableFrom(typeToDecorate))
                {
                    return DecorateWrappedWebElement(typeToDecorate, locator, bys);
                }

                if (typeToDecorate.IsGenericType)
                {
                    var genericTypeDefinition = typeToDecorate.GetGenericTypeDefinition();
                    var genericTypeArgument = typeToDecorate.GenericTypeArguments.Single();

                    if (typeof(IEnumerable<>).IsAssignableFrom(genericTypeDefinition))
                    {
                        if (typeof(IWebElement).IsAssignableFrom(genericTypeArgument))
                        {
                            return WebElementEnumerableProxy.Create(locator, bys);
                        }

                        if (typeof(IWrapsElement).IsAssignableFrom(genericTypeArgument))
                        {
                            var method = typeof(ProxyPageObjectMemberDecorator).GetMethod("DecorateEnumerableWrappedElement", new[] { typeof(IElementLocator), typeof(IEnumerable<By>) });
                            method = method.MakeGenericMethod(genericTypeArgument);
                            var element = method.Invoke(this, new object[] { locator, bys });

                            return element;
                        }
                    }
                }
            }

            return null;
        }


        public IEnumerable<T> DecorateEnumerableWrappedElement<T>(IElementLocator locator, IEnumerable<By> bys)
        {
            return WebElementEnumerableProxy.Create(locator, bys)
                .Select(webElement =>
                {
                    var wrappedElement = _elementActivator.Create<T>(webElement);
                    var wrappedElementProperty = wrappedElement.GetType()
                        .GetMember("WrappedElement")
                        .Single() as PropertyInfo;

                    if (!wrappedElementProperty.CanWrite)
                    {
                        throw new Exception($"Can't write to {wrappedElementProperty}");
                    }

                    wrappedElementProperty.SetValue(wrappedElement, webElement);

                    _factory.InitElements(wrappedElement, new DefaultElementLocator(webElement), this);

                    return wrappedElement;
                });
        }


        private object DecorateWebElement(IElementLocator locator, IEnumerable<By> bys)
        {
            return WebElementProxy.Create(locator, bys, _webDriverWaiter);
        }

        private object DecorateWrappedWebElement(Type typeToDecorate, IElementLocator locator, IEnumerable<By> bys)
        {
            var element = WebElementProxy.Create(locator, bys, _webDriverWaiter);

            var wrappedElement = _elementActivator.Create(typeToDecorate, element);
            var wrappedElementProperty = wrappedElement.GetType()
                .GetMember("WrappedElement")
                .Single() as PropertyInfo;

            //TODO: pretify this
            if (!wrappedElementProperty.CanWrite)
            {
                throw new Exception($"Can't write to {wrappedElementProperty}");
            }

            wrappedElementProperty.SetValue(wrappedElement, element);

            _factory.InitElements(wrappedElement, new DefaultElementLocator(element), this);

            return wrappedElement;
        }




        public bool TryCanDecorate(MemberInfo member, out Type type)
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