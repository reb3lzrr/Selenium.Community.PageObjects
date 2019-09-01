using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.Community.PageObjects.Tests.Autofixture;

namespace Selenium.Community.PageObjects.Tests
{
    public class DefaultElementLocatorTests
    {
        private readonly IEnumerable<By> _bys = new[]
        {
            By.ClassName("a"),
            By.ClassName("b")
        };

        [Test]
        public void Ctor_ArgumentNullExceptions()
        {
            new Action(() => new DefaultElementLocator(null)).Should()
                .Throw<ArgumentNullException>();
        }

        [Theory]
        [AutoDomainData]
        public void LocateElement_ArgumentNullExceptions(DefaultElementLocator sut)
        {
            new Action(() => sut.LocateElement(null)).Should().Throw<ArgumentNullException>();
        }


        [Theory]
        [AutoDomainData]
        public void LocateElement_CallsSearchContext([Frozen] Mock<ISearchContext> searchContextMock,
            [Frozen]Mock<IWebElement> elementMock,
            DefaultElementLocator sut)
        {
            searchContextMock.Setup(x => x.FindElements(It.IsAny<By>())).Returns(new ReadOnlyCollection<IWebElement>(new[] { elementMock.Object }.ToList()));

            sut.LocateElement(_bys);

            searchContextMock.Verify(x => x.FindElements(It.IsAny<By>()), Times.Once());
        }

        [Theory]
        [AutoDomainData]
        public void LocateElement_IteratesBysUntilAnyWasFound(
            [Frozen] Mock<ISearchContext> searchContextMock,
            Mock<IWebElement> webElementMock,
            DefaultElementLocator sut)
        {
            searchContextMock.Setup(x => x.FindElements(_bys.First()))
                .Throws(new NoSuchElementException());
            searchContextMock.Setup(x => x.FindElements(_bys.Last()))
                .Returns(new ReadOnlyCollection<IWebElement>(new[] { webElementMock.Object }));

            sut.LocateElement(_bys).Should().Be(webElementMock.Object);

            searchContextMock.Verify(x => x.FindElements(_bys.First()), Times.AtLeastOnce());
            searchContextMock.Verify(x => x.FindElements(_bys.Last()), Times.AtLeastOnce());
        }

        [Theory]
        [AutoDomainData]
        public void LocateElement_ThrowsExceptionWhenNothingIsFound(
            [Frozen] Mock<ISearchContext> searchContextMock,
            DefaultElementLocator sut)
        {
            searchContextMock.Setup(x => x.FindElements(_bys.First()))
                .Throws(new NoSuchElementException());
            searchContextMock.Setup(x => x.FindElements(_bys.Last()))
                .Throws(new NoSuchElementException());

            Action action = () => sut.LocateElement(_bys);
            action.Should().Throw<NoSuchElementException>();
        }

        [Theory]
        [AutoDomainData]
        public void LocateElements_ArgumentNullExceptions(DefaultElementLocator sut)
        {
            new Action(() => sut.LocateElements(null)).Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [AutoDomainData]
        public void LocateElements_CallsSearchContext([Frozen] Mock<ISearchContext> searchContextMock,
            [Frozen]Mock<IWebElement> elementMock,
            DefaultElementLocator sut)
        {
            searchContextMock.Setup(x => x.FindElements(It.IsAny<By>())).Returns(new ReadOnlyCollection<IWebElement>(new[] { elementMock.Object }.ToList()));

            sut.LocateElements(_bys);

            searchContextMock.Verify(x => x.FindElements(It.IsAny<By>()), Times.Exactly(_bys.Count()));
        }

        [Theory]
        [AutoDomainData]
        public void LocateElements_IteratesBysUntilAnyWasFound(
            [Frozen] Mock<ISearchContext> searchContextMock,
            Mock<IWebElement> webElementMock,
            DefaultElementLocator sut)
        {
            searchContextMock.Setup(x => x.FindElements(_bys.First()))
                .Throws(new NoSuchElementException());
            searchContextMock.Setup(x => x.FindElements(_bys.Last()))
                .Returns(new ReadOnlyCollection<IWebElement>(new[] { webElementMock.Object }));

            sut.LocateElements(_bys).Should().OnlyContain(x => x == webElementMock.Object);

            searchContextMock.Verify(x => x.FindElements(_bys.First()), Times.AtLeastOnce());
            searchContextMock.Verify(x => x.FindElements(_bys.Last()), Times.AtLeastOnce());
        }

        [Theory]
        [AutoDomainData]
        public void LocateElements_ReturnsEmptyListWhenNothingIsFound(
            [Frozen] Mock<ISearchContext> searchContextMock,
            DefaultElementLocator sut)
        {
            searchContextMock.Setup(x => x.FindElements(_bys.First()))
                .Throws(new NoSuchElementException());
            searchContextMock.Setup(x => x.FindElements(_bys.Last()))
                .Throws(new NoSuchElementException());

            sut.LocateElements(_bys).Should().BeEmpty();
        }
    }
}
