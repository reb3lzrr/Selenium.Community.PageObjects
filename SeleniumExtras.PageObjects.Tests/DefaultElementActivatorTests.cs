using System;
using FluentAssertions;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests
{
    public class DefaultElementActivatorTests
    {
        [Fact]
        public void NotEveryParameters_Throws()
        {
            var sut = new DefaultElementActivator();

            var action = new Action(() => sut.Create<TestClass>(new MyImplementation()));

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NotEveryParameters()
        {
            var sut = new DefaultElementActivator();

            var result = sut.Create<TestClass>(new MyClass());

            result.MyClass.Should().NotBeNull();
        }

        [Fact]
        public void TooManyParameters()
        {
            var sut = new DefaultElementActivator();

            var result = sut.Create<TestClass>(new MyClass(), new MyImplementation(), "NotUsed");

            result.MyClass.Should().NotBeNull();
        }


        [Fact]
        public void IsGreedy()
        {
            var sut = new DefaultElementActivator();

            var result = sut.Create<TestClass>(new MyClass(), new MyImplementation());

            result.MyClass.Should().NotBeNull();
            result.MyImplementation.Should().NotBeNull();
        }

        private interface MyInterface
        {
            
        }

        private class MyImplementation : MyInterface
        {

        }

        private class MyClass
        {

        }

        private class TestClass
        {
            public MyClass MyClass { get; }
            public MyImplementation MyImplementation { get; }

            public TestClass(MyClass myClass)
            {
                MyClass = myClass;
            }

            public TestClass(MyClass myClass, MyImplementation myImplementation)
            {
                MyClass = myClass;
                MyImplementation = myImplementation;
            }
        }
    }
}
