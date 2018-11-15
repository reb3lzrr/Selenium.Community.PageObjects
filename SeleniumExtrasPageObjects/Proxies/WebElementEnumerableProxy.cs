using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumExtras.PageObjects.Proxies
{
    public class WebElementEnumerableProxy : DispatchProxy
    {
        private IElementLocator _elementLocator;
        private IEnumerable<By> _bys;


        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var exceptions = new List<Exception>();
            var webElements = _elementLocator.LocateElements(_bys);
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    return targetMethod.Invoke(webElements, args);
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
                    webElements = _elementLocator.LocateElements(_bys);
                }
            }

            throw new AggregateException(exceptions);
        }

        public static IEnumerable<IWebElement> Create(IElementLocator elementLocator, IEnumerable<By> bys)
        {
            var proxiedObject = DispatchProxy.Create<IEnumerable<IWebElement>, WebElementEnumerableProxy>();
            (proxiedObject as WebElementEnumerableProxy)?.SetParameters(elementLocator, bys);

            return proxiedObject;
        }

        public void SetParameters(IElementLocator elementLocator, IEnumerable<By> bys)
        {
            _elementLocator = elementLocator;
            _bys = bys;
        }
    }
}
