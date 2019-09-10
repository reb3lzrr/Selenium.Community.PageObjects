using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// Provides waiting methodes for collection
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Repeatedly tries a collection to a given condition until that condition is met.
        /// </summary>
        /// <param name="enumerable">The collection</param>
        /// <param name="condition">The condition</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when either enumerable or condition are null</exception>
        /// <exception cref="WebDriverTimeoutException">Thrown when the condition is not met within the configured timeout ( 10 seconds default )</exception>
        public static IEnumerable<T> WaitUntil<T>(this IEnumerable<T> enumerable, Func<IEnumerable<T>, bool> condition)
        {
            return enumerable.WaitUntil(condition, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(50));
        }

        /// <summary>
        /// Repeatedly tries a collection to a given condition until that condition is met.
        /// </summary>
        /// <param name="enumerable">The collection</param>
        /// <param name="condition">The condition</param>
        /// <param name="timeout">The period of time after which to stop trying</param>
        /// <param name="pollingInterval">The period of time after to try again</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when either enumerable or condition are null</exception>
        /// <exception cref="WebDriverTimeoutException">Thrown when the condition is not met within the configured timeout ( 10 seconds default )</exception>
        public static IEnumerable<T> WaitUntil<T>(this IEnumerable<T> enumerable, Func<IEnumerable<T>, bool> condition,
            TimeSpan timeout, TimeSpan pollingInterval)
        {
            enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
            condition = condition ?? throw new ArgumentNullException(nameof(condition));

            var waiter = new DefaultWait<IEnumerable<T>>(enumerable)
            {
                Timeout = timeout,
                PollingInterval = pollingInterval
            };

            return waiter.Until(elements => condition(elements) ? elements : null);
        }
    }
}
