using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.PageObjects.Tests.Page;
using Xunit.Sdk;

namespace SeleniumExtras.PageObjects.Tests
{
    public class UseContainer : DataAttribute
    {
        private static IContainer Container { get; }

        static UseContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ChromeDriver>()
                .WithParameter("service", ChromeDriverService.CreateDefaultService("."))
                .As<IWebDriver>()
                .As<IJavaScriptExecutor>()
                .SingleInstance();
            builder.RegisterType<PageObjectFactory>();
            builder.RegisterType<TestPageObject>();        

            Container = builder.Build();
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            yield return testMethod.GetParameters()
                .Select(x => x.ParameterType)
                .Select(x => Container.Resolve(x))
                .ToArray();
        }
    }
}
