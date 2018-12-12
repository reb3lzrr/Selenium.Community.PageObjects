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

        public object Decorate(Type typeToDecorate, IEnumerable<By> bys, IElementLocator elementLocator)
        {
            if (typeof(IWebElement).IsAssignableFrom(typeToDecorate))
            {
                return DecorateWebElement(elementLocator, bys);
            }

            if (typeof(IWrapsElement).IsAssignableFrom(typeToDecorate))
            {
                return DecorateWrappedWebElement(typeToDecorate, elementLocator, bys);
            }

            if (typeToDecorate.IsGenericType)
            {
                var genericTypeDefinition = typeToDecorate.GetGenericTypeDefinition();
                var genericTypeArgument = typeToDecorate.GenericTypeArguments.Single();

                if (typeof(IEnumerable<>).IsAssignableFrom(genericTypeDefinition))
                {
                    if (typeof(IWebElement).IsAssignableFrom(genericTypeArgument))
                    {
                        return WebElementEnumerableProxy.Create(elementLocator, bys);
                    }

                    if (typeof(IWrapsElement).IsAssignableFrom(genericTypeArgument))
                    {
                        var method = typeof(ProxyPageObjectMemberDecorator).GetMethod(nameof(ProxyPageObjectMemberDecorator.DecorateEnumerableWrappedElement), new[] { typeof(IElementLocator), typeof(IEnumerable<By>) });
                        method = method.MakeGenericMethod(genericTypeArgument);
                        var element = method.Invoke(this, new object[] { elementLocator, bys });

                        return element;
                    }
                }
            }

            //TODO: find or make exception
            throw new Exception($"Can't decorate {typeToDecorate}");
        }

        public IEnumerable<T> DecorateEnumerableWrappedElement<T>(IElementLocator elementLocator, IEnumerable<By> bys)
        {
            return WebElementEnumerableProxy.Create(elementLocator, bys)
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

                    _factory.InitElements(wrappedElement, new DefaultElementLocator(webElement));

                    return wrappedElement;
                });
        }


        private object DecorateWebElement(IElementLocator elementLocator, IEnumerable<By> bys)
        {
            return WebElementProxy.Create(elementLocator, bys, _webDriverWaiter);
        }

        private object DecorateWrappedWebElement(Type typeToDecorate, IElementLocator elementLocator, IEnumerable<By> bys)
        {
            var element = WebElementProxy.Create(elementLocator, bys, _webDriverWaiter);

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

            _factory.InitElements(wrappedElement, new DefaultElementLocator(element));

            return wrappedElement;
        }
    }
}