using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Selenium.Community.PageObjects.Tests
{
    public class FindsByAttributeTests
    {
        [Test]
        public void FindsBy_Class()
        {
            new FindsByAttribute(How.ClassName, "a").ByFinder().Should().Be(By.ClassName("a"));
        }

        [Test]
        public void FindsBy_Css()
        {
            new FindsByAttribute(How.CssSelector, "a").ByFinder().Should().Be(By.CssSelector("a"));          
        }

        [Test]
        public void FindsBy_Id()
        {
            new FindsByAttribute(How.Id, "a").ByFinder().Should().Be(By.Id("a"));
        }

        [Test]
        public void FindsBy_LinkText()
        {
            new FindsByAttribute(How.LinkText, "a").ByFinder().Should().Be(By.LinkText("a"));
        }

        [Test]
        public void FindsBy_PartialLinkText()
        {
            new FindsByAttribute(How.PartialLinkText, "a").ByFinder().Should().Be(By.PartialLinkText("a"));
        }

        [Test]
        public void FindsBy_Name()
        {
            new FindsByAttribute(How.Name, "a").ByFinder().Should().Be(By.Name("a"));
        }

        [Test]
        public void FindsBy_TagName()
        {
            new FindsByAttribute(How.TagName, "a").ByFinder().Should().Be(By.TagName("a"));
        }

        [Test]
        public void FindsBy_XPath()
        {
            new FindsByAttribute(How.XPath, "a").ByFinder().Should().Be(By.XPath("a"));
        }
    }
}
