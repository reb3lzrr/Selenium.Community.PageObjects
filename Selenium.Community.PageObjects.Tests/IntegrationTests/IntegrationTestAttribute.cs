using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutoFixture.NUnit3;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Selenium.Community.PageObjects.Tests.IntegrationTests
{
    public class IntegrationTestAttribute : Attribute, ITestBuilder
    {
        private readonly FixedNameTestMethodBuilder _testMethodBuilder;

        public static  IContainer Container; 

        public IntegrationTestAttribute()
        {
            _testMethodBuilder = new FixedNameTestMethodBuilder();
        }

        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {

            return new[] {_testMethodBuilder.Build(method, suite, GetParameterValues(method), 0)};
        }

        private IEnumerable<object> GetParameterValues(IMethodInfo method)
        {
            return method.GetParameters().Select(ResolveParameter);
        }

        private object ResolveParameter(IParameterInfo parameterInfo)
        {
            return Container.Resolve(parameterInfo.ParameterType);
        }

    }
}
