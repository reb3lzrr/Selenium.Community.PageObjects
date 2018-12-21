using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects.Tests.Autofixture;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests
{
    public class DefaultElementLocatorTests
    {
        private readonly IEnumerable<By> _bys = new[]
        {
            By.ClassName("a"),
            By.ClassName("b")
        };

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
