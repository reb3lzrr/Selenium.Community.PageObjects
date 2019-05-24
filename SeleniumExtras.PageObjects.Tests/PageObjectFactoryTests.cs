using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects.Tests.Autofixture;
using System;
using System.Collections.Generic;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests
{
    public class PageObjectFactoryTests
    {
        [Theory]
        [AutoDomainData]
        public void PageObjectFactory_DoesNotDecorateUndecoratedMembers([Frozen] Mock<IPageObjectMemberDecorator> memberDecoratorMock,
            PageObjectFactory pageObjectFactory)
        {
            pageObjectFactory.InitElements(new PageObjectWithUndecoratedMembers());

            memberDecoratorMock.Verify(x => x.Decorate(It.IsAny<Type>(),
                It.IsAny<IEnumerable<By>>(),
                It.IsAny<IElementLocator>()), Times.Never());
        }

        [Theory]
        [AutoDomainData]
        public void PageObjectFactory_DecoratesFields([Frozen] Mock<IPageObjectMemberDecorator> memberDecoratorMock,
            PageObjectFactory pageObjectFactory,
            PageObjectWithFields pageObject,
            int value)
        {
            memberDecoratorMock.Setup(x => x.Decorate(typeof(int),
                    It.IsAny<IEnumerable<By>>(),
                    It.IsAny<IElementLocator>()))
                .Returns(value);

            pageObjectFactory.InitElements(pageObject);

            memberDecoratorMock.Verify(x => x.Decorate(typeof(int),
                It.IsAny<IEnumerable<By>>(),
                It.IsAny<IElementLocator>()), Times.Exactly(4));

            pageObject.PublicField.Should().Be(value);
            pageObject.PrivateFieldValue().Should().Be(value);
            pageObject.ProtectedFieldValue().Should().Be(value);
            pageObject.InternalFieldValue().Should().Be(value);
        }

       
        [Theory]
        [AutoDomainData]
        public void PageObjectFactory_DecoratesProperties([Frozen] Mock<IPageObjectMemberDecorator> memberDecoratorMock,
            PageObjectFactory pageObjectFactory,
            PageObjectWithProperties pageObject,
            int value)
        {
            memberDecoratorMock.Setup(x => x.Decorate(typeof(int),
                    It.IsAny<IEnumerable<By>>(),
                    It.IsAny<IElementLocator>()))
                .Returns(value);

            pageObjectFactory.InitElements(pageObject);

            memberDecoratorMock.Verify(x => x.Decorate(typeof(int),
                It.IsAny<IEnumerable<By>>(),
                It.IsAny<IElementLocator>()), Times.Exactly(4));

            pageObject.PublicProperty.Should().Be(value);
            pageObject.PrivatePropertyValue().Should().Be(value);
            pageObject.ProtectedPropertyValue().Should().Be(value);
            pageObject.InternalPropertyValue().Should().Be(value);
        }
    }

    public class PageObjectWithUndecoratedMembers
    {
        public int UndecoratedIntProperty { get; set; }
        public int UndecoratedLongField;
    }

    public class PageObjectWithProperties
    {
        [FindsBy(How.ClassName, "a")]
        public int PublicProperty { get; set; }

        [FindsBy(How.ClassName, "a")]
        private int PrivateProperty { get; set; }

        [FindsBy(How.ClassName, "a")]
        protected int ProtectedProperty { get; set; }

        [FindsBy(How.ClassName, "a")]
        internal int InternalProperty { get; set; }

        public int PrivatePropertyValue()
        {
            return PrivateProperty;
        }

        public int ProtectedPropertyValue()
        {
            return ProtectedProperty;
        }

        public int InternalPropertyValue()
        {
            return InternalProperty;
        }
    }

    public class PageObjectWithFields
    {
        [FindsBy(How.ClassName, "a")]
        public int PublicField;

        [FindsBy(How.ClassName, "a")]
        private int PrivateField = -1;

        [FindsBy(How.ClassName, "a")]
        protected int ProtectedField;

        [FindsBy(How.ClassName, "a")]
        internal int InternalField = -1;

        public int PrivateFieldValue()
        {
            return PrivateField;
        }

        public int ProtectedFieldValue()
        {
            return ProtectedField;
        }

        public int InternalFieldValue()
        {
            return InternalField;
        }
    }

}
