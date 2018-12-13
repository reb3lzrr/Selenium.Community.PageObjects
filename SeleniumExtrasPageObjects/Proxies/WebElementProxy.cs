using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium;

namespace SeleniumExtras.PageObjects.Proxies
{
    public class WebElementProxy : DispatchProxy
    {
        private IElementLocator _elementLocator;
        private IEnumerable<By> _bys;
        private IWebElement _proxiedObject;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return Invoke(targetMethod, args, true);

        }

        private object Invoke(MethodInfo targetMethod, object[] args, bool reAttach)
        {
            if (_proxiedObject == null)
            {
                _proxiedObject = _elementLocator.LocateElement(_bys);
            }

            try
            {
                return targetMethod.Invoke(_proxiedObject, args);
            }
            catch (TargetInvocationException tie)
            {
                if (reAttach)
                {
                    _proxiedObject = null;
                    return Invoke(targetMethod, args, false);
                }
                throw tie.InnerException;
            }
        }

        public static IWebElement Create(IElementLocator elementLocator, IEnumerable<By> bys)
        {
            var proxiedObject = DispatchProxy.Create<IWebElement, WebElementProxy>();
            (proxiedObject as WebElementProxy)?.SetParameters(elementLocator, bys);

            return proxiedObject;
        }

        public void SetParameters(IElementLocator elementLocator, IEnumerable<By> bys)
        {
            _elementLocator = elementLocator;
            _bys = bys;
        }
    }
}
