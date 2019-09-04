using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;

namespace Selenium.Community.PageObjects.Tests.Autofixture
{
    internal class AutoDomainDataAttribute : InlineAutoDataAttribute
    {
        public static Func<IFixture> fixtureFactory = () =>
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization());
            fixture.Customize<PageObjectFactory>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
            return fixture;
        };

        public AutoDomainDataAttribute(params object[] parameters) : base(fixtureFactory, parameters)
        {

        }
    }
}