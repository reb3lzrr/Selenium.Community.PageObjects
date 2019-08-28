using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using Selenium.Community.PageObjects.Tests.Autofixture;

namespace Selenium.Community.PageObjects.Tests
{
    public class ProxyPageObjectMemberDecoratorTests
    {
        public class WrapsElement : IWrapsElement
        {
            public WrapsElement(IWebElement wrappedElement)
            {
                WrappedElement = wrappedElement;
            }

            public IWebElement WrappedElement { get; private set; }
        }

        [Theory]
        [AutoDomainData]
        public void Decorate_IWebElement(
            IEnumerable<By> bys,
            IElementLocator elementLocator,
            [Frozen] Mock<IWebDriver> webDriverMock,
            [Frozen] WrapsElement wrapsElement,
            [Frozen] Mock<IElementActivator> elementActivatorMock,
            ProxyPageObjectMemberDecorator sut)
        {
            var result = sut.Decorate(typeof(IWebElement), bys, elementLocator);

            result.GetType().Name.Should().Contain("Proxy");
        }

        [Theory]
        [AutoDomainData(typeof(IEnumerable<IWebElement>))]
        public void Decorate_IWebElementEnumerable(
            Type type,
            IEnumerable<By> bys,
            IElementLocator elementLocator,
            [Frozen] Mock<IWebDriver> webDriverMock,
            [Frozen] WrapsElement wrapsElement,
            [Frozen] Mock<IElementActivator> elementActivatorMock,
            ProxyPageObjectMemberDecorator sut)
        {
            var result = (IEnumerable<IWebElement>)sut.Decorate(type, bys, elementLocator);

            foreach (var member in result)
            {
                member.Should().GetType().Name.Should().Contain("Proxy");
            }
        }

        [Theory]
        [AutoDomainData(typeof(ICollection<IWebElement>))]
        [AutoDomainData(typeof(IList<IWebElement>))]
        [AutoDomainData(typeof(IReadOnlyCollection<IWebElement>))]
        [AutoDomainData(typeof(IWebElement[]))]
        [AutoDomainData(typeof(List<IWebElement>))]
        public void Decorate_IWebElementEnumerable_Unsupported(
            Type type,
            IEnumerable<By> bys,
            IElementLocator elementLocator,
            [Frozen] Mock<IWebDriver> webDriverMock,
            [Frozen] WrapsElement wrapsElement,
            [Frozen] Mock<IElementActivator> elementActivatorMock,
            ProxyPageObjectMemberDecorator sut)
        {
            new Action(() => sut.Decorate(type, bys, elementLocator)).Should().Throw<Exception>();
        }

        [Theory]
        [AutoDomainData]
        public void Decorate_IWrapsElementImplementation(
            IEnumerable<By> bys,
            IElementLocator elementLocator,
            [Frozen] Mock<IWebDriver> webDriverMock,
            [Frozen] WrapsElement wrapsElement,
            [Frozen] Mock<IElementActivator> elementActivatorMock,
            ProxyPageObjectMemberDecorator sut)
        {
            elementActivatorMock
                .Setup(x => x.Create(typeof(WrapsElement), It.IsAny<object[]>()))
                .Callback(new Action<Type, object[]>((typeToActivate, args) =>
                {
                    args[0].GetType().Name.Should().Contain("Proxy");
                }))
                .Returns(wrapsElement);

            var result = sut.Decorate(typeof(WrapsElement), bys, elementLocator);

            result.Should().BeOfType<WrapsElement>();
            elementActivatorMock
                .Verify(x => x.Create(typeof(WrapsElement), It.IsAny<object[]>()), Times.Once());
        }

        [Theory]
        [AutoDomainData(typeof(IEnumerable<WrapsElement>))]
        public void Decorate_IWrapsElementIEnumerableImplementation(
            Type type,
            IEnumerable<By> bys,
            IElementLocator elementLocator,
            [Frozen] WrapsElement wrapsElement,
            [Frozen] Mock<IElementActivator> elementActivatorMock,
            ProxyPageObjectMemberDecorator sut)
        {
            elementActivatorMock
                .Setup(x => x.Create(type, It.IsAny<object[]>()))
                .Returns(wrapsElement);

            new Action(() => sut.Decorate(type, bys, elementLocator)).Should().NotThrow();
        }

        [Theory]
        [AutoDomainData(typeof(ICollection<WrapsElement>))]
        [AutoDomainData(typeof(IList<WrapsElement>))]
        [AutoDomainData(typeof(IReadOnlyCollection<WrapsElement>))]
        [AutoDomainData(typeof(WrapsElement[]))]
        [AutoDomainData(typeof(List<WrapsElement>))]
        public void Decorate_IWrapsElementIEnumerableImplementation_Unsupported(
            Type type,
            IEnumerable<By> bys,
            IElementLocator elementLocator,
            [Frozen] WrapsElement wrapsElement,
            [Frozen] Mock<IElementActivator> elementActivatorMock,
            ProxyPageObjectMemberDecorator sut)
        {
            elementActivatorMock
                .Setup(x => x.Create(type, It.IsAny<object[]>()))
                .Returns(wrapsElement);

            new Action(() => sut.Decorate(type, bys, elementLocator)).Should().Throw<Exception>();
        }
    }
}
