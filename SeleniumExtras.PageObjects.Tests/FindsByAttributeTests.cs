using FluentAssertions;
using OpenQA.Selenium;
using Xunit;

namespace SeleniumExtras.PageObjects.Tests
{

    public class FindsByAttributeTests
    {
        [Fact]
        public void FindsBy_Class()
        {
            new FindsByAttribute(How.ClassName, "a").Finder().Should().Be(By.ClassName("a"));
        }

        [Fact]
        public void FindsBy_Css()
        {
            new FindsByAttribute(How.CssSelector, "a").Finder().Should().Be(By.CssSelector("a"));          
        }

        [Fact]
        public void FindsBy_Id()
        {
            new FindsByAttribute(How.Id, "a").Finder().Should().Be(By.Id("a"));
        }

        [Fact]
        public void FindsBy_LinkText()
        {
            new FindsByAttribute(How.LinkText, "a").Finder().Should().Be(By.LinkText("a"));
        }

        [Fact]
        public void FindsBy_PartialLinkText()
        {
            new FindsByAttribute(How.PartialLinkText, "a").Finder().Should().Be(By.PartialLinkText("a"));
        }

        [Fact]
        public void FindsBy_Name()
        {
            new FindsByAttribute(How.Name, "a").Finder().Should().Be(By.Name("a"));
        }

        [Fact]
        public void FindsBy_TagName()
        {
            new FindsByAttribute(How.TagName, "a").Finder().Should().Be(By.TagName("a"));
        }

        [Fact]
        public void FindsBy_XPath()
        {
            new FindsByAttribute(How.XPath, "a").Finder().Should().Be(By.XPath("a"));
        }
    }
}
