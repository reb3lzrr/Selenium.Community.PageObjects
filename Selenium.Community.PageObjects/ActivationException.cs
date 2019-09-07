using System;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// An exception thrown when an PageObject cannot be activated
    /// </summary>
    public class ActivationException : Exception
    {
        internal ActivationException(string message) : base(message)
        {
        }
    }
}