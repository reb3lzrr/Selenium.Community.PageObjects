using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects.Proxies;
using SeleniumExtras.PageObjects.Tests.Autofixture;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests.Proxies
{
    public class WebElementEnumerableProxyTests
    {
        [Theory]
        [AutoDomainData]
        public void IsLazy(
            Mock<IWebElement> webElementMock,
            Mock<IElementLocator> elementLocatorMock,
            ICollection<By> bys)
        {
            elementLocatorMock.Setup(x => x.LocateElements(bys))
                .Returns(new[] { webElementMock.Object });

            var proxy = WebElementEnumerableProxy.Create(elementLocatorMock.Object, bys);


            elementLocatorMock.Verify(x => x.LocateElements(bys), Times.Never());

            proxy.Count();

            elementLocatorMock.Verify(x => x.LocateElements(bys), Times.Once());
        }
    }
}
