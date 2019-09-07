using System;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// An exception thrown when a member which is marked by a <see cref="ByAttribute"/> could not be decorated
    /// </summary>
    public class DecorationException : Exception
    {
        internal DecorationException(string message) : base(message)
        {
        }
    }
}