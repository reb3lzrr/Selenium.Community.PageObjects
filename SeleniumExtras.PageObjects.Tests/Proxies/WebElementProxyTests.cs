using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects.Proxies;
using SeleniumExtras.PageObjects.Tests.Autofixture;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests.Proxies
{
    public class WebElementProxyTests
    {
        [Theory]
        [AutoDomainData]
        public void IsLazy(Mock<IElementLocator> elementLocatorMock,
            ICollection<By> bys)
        {
            var proxy = WebElementProxy.Create(elementLocatorMock.Object, bys);

            elementLocatorMock.Verify(x => x.LocateElement(bys), Times.Never());

            proxy.Click();

            elementLocatorMock.Verify(x => x.LocateElement(bys), Times.Once());
        }

        [Theory]
        [AutoDomainData]
        public void Caches(Mock<IElementLocator> elementLocatorMock,
            ICollection<By> bys)
        {
            var proxy = WebElementProxy.Create(elementLocatorMock.Object, bys);

            proxy.Click();
            proxy.Click();
            proxy.Click();

            elementLocatorMock.Verify(x => x.LocateElement(bys), Times.Once());
        }

        [Theory]
        [AutoDomainData]
        public void InvocationTrows_Retry(Mock<IElementLocator> elementLocatorMock,
            Mock<IWebElement> webElementMock,
            ICollection<By> bys)
        {
            var counter = 0;
            elementLocatorMock.Setup(x => x.LocateElement(bys))
                .Returns(webElementMock.Object);
            webElementMock.Setup(x => x.Click())
                .Callback(() =>
                {
                    if (++counter == 3)
                    {
                        throw new StaleElementReferenceException();
                    }
                });

            var proxy = WebElementProxy.Create(elementLocatorMock.Object, bys);

            proxy.Click();
            proxy.Click();

            elementLocatorMock.Verify(x => x.LocateElement(bys), Times.Once());

            //Thrid call will cause a StaleElementReferenceException
            proxy.Click();

            elementLocatorMock.Verify(x => x.LocateElement(bys), Times.Exactly(2));
        }

        [Theory]
        [AutoDomainData]
        public void InvocationTrows_Rethrows(Mock<IElementLocator> elementLocatorMock,
            Mock<IWebElement> webElementMock,
            ICollection<By> bys)
        {
            var counter = 0;
            elementLocatorMock.Setup(x => x.LocateElement(bys))
                .Returns(webElementMock.Object);
            webElementMock.Setup(x => x.Click())
                .Callback(() =>
                {
                    if (++counter >= 3)
                    {
                        throw new StaleElementReferenceException();
                    }
                });

            var proxy = WebElementProxy.Create(elementLocatorMock.Object, bys);

            proxy.Click();
            proxy.Click();

            //Thrid call will cause a StaleElementReferenceException
            Action action = () => proxy.Click();

            action.Should().Throw<StaleElementReferenceException>();
        }
    }
}
