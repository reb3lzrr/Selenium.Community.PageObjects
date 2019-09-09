using System;
using OpenQA.Selenium;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// An attribute that serves as the root for inheritance.
    /// <para>To be used in conjecture with <see cref="PageObjectFactory"></see>, as it will search for implementations of this 
    /// of <see cref="ByAttribute"/> which provide information how to locate the member's value accordingly
    /// </para>
    /// <para>
    /// The Default implementation is <see cref="FindsByAttribute"/>
    /// </para>
    /// </summary>
    public abstract class ByAttribute : Attribute
    {
        /// <summary>
        /// Gets an explicit <see cref="By"/> object used to locate the member with.
        /// </summary>
        public abstract By ByFinder();
    }
}