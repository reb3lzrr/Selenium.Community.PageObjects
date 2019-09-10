using FluentAssertions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.Community.PageObjects.Tests.Autofixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Selenium.Community.PageObjects.Tests
{
    public class WaitUntilTest
    {
        [Theory]
        [AutoDomainData]
        public void WaitUntil_CollectionArgumentNullExceptions(IEnumerable<int> collection)
        {
            collection = null;
            new Action(() => collection.WaitUntil(null)).Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [AutoDomainData]
        public void WaitUntil_ConditionArgumentNullExceptions(IEnumerable<int> collection)
        {
            new Action(() => collection.WaitUntil(null)).Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [AutoDomainData]
        public void WaitUntil_Retry(Mock<IWebElement> webElementMock)
        {
            webElementMock.SetupSequence(x => x.GetProperty("test"))
                .Returns("a")
                .Returns("a")
                .Returns("a")
                .Returns("b")
                .Returns("c");

            new[] { webElementMock.Object }
                .WaitUntil(x => x.All(y => y.GetProperty("test") == "b"));

            webElementMock.Object.GetProperty("test").Should().Be("c");
        }

        [Theory]
        [AutoDomainData]
        public void WaitUntil_TimeoutException(Mock<IWebElement> webElementMock)
        {
            webElementMock.Setup(x => x.GetProperty("test"))
                .Returns("a");

            new Action(() =>
            {
                new[] {webElementMock.Object}
                    .WaitUntil(x => x.All(y => y.GetProperty("test") == "b"), TimeSpan.FromMilliseconds(10),
                        TimeSpan.FromMilliseconds(1));
            }).Should().Throw<WebDriverTimeoutException>();
        }
    }
}