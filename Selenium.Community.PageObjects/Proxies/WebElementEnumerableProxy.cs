using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium;

namespace Selenium.Community.PageObjects.Proxies
{
    public class WebElementEnumerableProxy : DispatchProxy
    {
        private IElementLocator _elementLocator;
        private IEnumerable<By> _bys;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var webElements = _elementLocator.LocateElements(_bys);

            return targetMethod.Invoke(webElements, args);
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
