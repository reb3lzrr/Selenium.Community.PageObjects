using System;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// An interface describing an activator for members of a PageObject, to use with the <see cref="PageObjectFactory"/>.
    /// </summary>
    public interface IElementActivator
    {
        /// <summary>
        /// Creates an instance of <param name="type">a type</param>, given a set of <param name="parameters">constructor parameter values</param> 
        /// </summary>
        /// <param name="type">The type to instantiate</param>
        /// <param name="parameters">The constructor parameters for <param name="type">type</param></param>
        /// <returns>An instance of <see cref="type"/>type</returns>
        object Create(Type type, params object[] parameters);

        /// <summary>
        /// Creates an instance of <typeparam name="T">type T</typeparam>, given a set of <param name="parameters">constructor parameter values</param> 
        /// </summary>
        /// <param name="parameters">The constructor parameters for <typeparam name="T">type T</typeparam></param>
        /// <returns>An instance of <typeparam name="T">type T</typeparam></returns>
        T Create<T>(params object[] parameters);
    }
}
