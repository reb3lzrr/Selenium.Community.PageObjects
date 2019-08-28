using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Selenium.Community.PageObjects.Tests.Autofixture;

namespace Selenium.Community.PageObjects.Tests
{
    public class DefaultElementActivatorTests
    {
        [Test]
        public void DefaultElementActivator_Create_Throws()
        {
            var sut = new DefaultElementActivator();
            var action = new Action(() => sut.Create<ClassWithMultipleConstructors>());

            action.Should().Throw<ActivationException>().WithMessage($"Unable to activate type {typeof(ClassWithMultipleConstructors)}. No matching constructor was found with provided parameters {string.Join(", ", new object[0].Select(x => x.GetType().ToString()))}");
        }

        [Theory]
        [AutoDomainData]
        public void DefaultElementActivator_Create_ViaContstructorParameters(Class1 class1)
        {
            var defaultElementActivator = new DefaultElementActivator(class1);
            var instance = defaultElementActivator.Create<ClassWithMultipleConstructors>();

            instance.Class1.Should().Be(class1);
        }

        [Theory]
        [AutoDomainData]
        public void DefaultElementActivator_Create_ViaParameters(Class1 class1)
        {
            var defaultElementActivator = new DefaultElementActivator();
            var instance = defaultElementActivator.Create<ClassWithMultipleConstructors>(class1);

            instance.Class1.Should().Be(class1);
        }

        [Theory]
        [AutoDomainData]
        public void DefaultElementActivator_Create_IsGreedy(Class1 class1, Class2 class2)
        {
            var sut = new DefaultElementActivator();
            var instance = sut.Create<ClassWithMultipleConstructors>(class1, class2);

            instance.Class1.Should().Be(class1);
            instance.Interface1.Should().Be(class1);
            instance.Interface2.Should().Be(class2);
        }

        [Theory]
        [AutoDomainData]
        public void DefaultElementActivator_Create_MatchesFirstParameter(Class1 class1, Class2 class2)
        {
            var sut = new DefaultElementActivator();
            var instance = sut.Create<ClassWithMultipleConstructors>(class2, class1);

            instance.Class1.Should().Be(class1);
            instance.Interface1.Should().Be(class2);
            instance.Interface2.Should().Be(class2);
        }


        [Theory]
        [AutoDomainData]
        public void DefaultElementActivator_Create_MatchesFirstParameter_ViaConstructorParameter(Class1 class1, Class2 class2)
        {
            var sut = new DefaultElementActivator(class1);
            var instance = sut.Create<ClassWithMultipleConstructors>(class2);

            instance.Class1.Should().Be(class1);
            instance.Interface1.Should().Be(class2);
            instance.Interface2.Should().Be(class2);
        }

        public class ClassWithMultipleConstructors
        {
            public Class1 Class1 { get; }
            public IInterface1 Interface1 { get; }
            public IInterface2 Interface2 { get; }

            public ClassWithMultipleConstructors(Class1 class1)
            {
                Class1 = class1;
            }

            public ClassWithMultipleConstructors(Class1 class1, IInterface1 interface1, IInterface2 interface2)
            {
                Class1 = class1;
                Interface1 = interface1;
                Interface2 = interface2;
            }
        }

        public class Class1 : IInterface1
        {

        }

        public class Class2 : IInterface1, IInterface2
        {

        }

        public interface IInterface1
        {

        }
        public interface IInterface2
        {

        }
    }
}
