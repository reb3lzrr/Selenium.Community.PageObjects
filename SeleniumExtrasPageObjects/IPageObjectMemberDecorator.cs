using System.Reflection;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// Interface describing how members of a class which represent elements in a Page Object
    /// are detected.
    /// </summary>
    public interface IPageObjectMemberDecorator
    {
        /// <summary>
        /// Locates an element or list of elements for a Page Object member.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/> containing information about
        /// a class's member.</param>
        /// <param name="locator">The <see cref="IElementLocator"/> used to locate elements.</param>
        /// <returns>The Page Object's member value</returns>
        object Decorate(MemberInfo member, IElementLocator locator);
    }
}