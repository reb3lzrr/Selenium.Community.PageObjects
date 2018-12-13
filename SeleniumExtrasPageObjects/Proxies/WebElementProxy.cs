using System;
using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumExtras.PageObjects.Proxies
{
    public class WebElementProxy : DispatchProxy
    {
        private IElementLocator _elementLocator;
        private IEnumerable<By> _bys;
        private DefaultWait<IWebDriver> _webDriverWaiter;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var exceptions = new List<Exception>();
            var proxiedObject = _webDriverWaiter.Until(x => _elementLocator.LocateElement(_bys));
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    return targetMethod.Invoke(proxiedObject, args);
                }
                catch (TargetInvocationException tie)
                {
                    if (tie.InnerException.GetType() != typeof(NoSuchElementException) &&
                        tie.InnerException.GetType() != typeof(StaleElementReferenceException) &&
                        i == 4)
                    {
                        throw;
                    }
                    exceptions.Add(tie);
                    proxiedObject = _webDriverWaiter.Until(x => _elementLocator.LocateElement(_bys));
                }
            }

            throw new AggregateException(exceptions);
        }

        public static IWebElement Create(IElementLocator elementLocator, IEnumerable<By> bys, DefaultWait<IWebDriver> webDriverWaiter)
        {
            var proxiedObject = DispatchProxy.Create<IWebElement, WebElementProxy>();
            (proxiedObject as WebElementProxy)?.SetParameters(elementLocator, bys, webDriverWaiter);

            return proxiedObject;
        }

        public void SetParameters(IElementLocator elementLocator, IEnumerable<By> bys, DefaultWait<IWebDriver> webDriverWaiter)
        {
            _elementLocator = elementLocator;
            _bys = bys;
            _webDriverWaiter = webDriverWaiter;
        }
    }
}
