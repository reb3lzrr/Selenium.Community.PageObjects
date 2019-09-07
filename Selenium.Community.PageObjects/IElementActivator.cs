using System;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// An interface describing an activator for members of a PageObject, to use with the <see cref="PageObjectFactory"/>.
    /// </summary>
    public interface IElementActivator
    {
        /// <summary>
        /// Creates an instance of a type, given a set of constructor parameter values
        /// </summary>
        /// <param name="type">The type to instantiate</param>
        /// <param name="parameters">The constructor parameters</param>
        /// <returns>An instance of type</returns>
        object Create(Type type, params object[] parameters);

        /// <summary>
        /// Creates an instance of a type, given a set of constructor parameter values
        /// </summary>
        /// <param name="parameters">The constructor parameters</param>
        /// <returns>An instance of <typeparam name="T">type T</typeparam></returns>
        T Create<T>(params object[] parameters);
    }
}
