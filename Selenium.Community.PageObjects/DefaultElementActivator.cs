using System;
using System.Linq;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// A default activator used by the <see cref="PageObjectFactory"/>
    /// </summary>
    public class DefaultElementActivator : IElementActivator
    {
        public object Create(Type type, params object[] parameters)
        {
            var availableTypesDictionary = parameters
                .Select(x => new { type = x.GetType(), obj = x })
                .Concat(parameters.SelectMany(parameter => parameter.GetType().GetInterfaces().Select(x => new { type = x, obj = parameter })))
                .GroupBy(x => x.type)
                .ToDictionary(x => x.Key, x => x.First().obj);

            var constructors = type
                .GetConstructors()
                .Select(x => new
                {
                    constructor = x,
                    constructorParameters = x.GetParameters(),
                    matchedParameters = x.GetParameters().Where(y => availableTypesDictionary.ContainsKey(y.ParameterType))
                        .Select(y => availableTypesDictionary[y.ParameterType])
                        .ToArray()
                });

            var invokaleConstructorInfo = constructors
                .Where(x => x.matchedParameters.Count() == x.constructorParameters.Length)
                .OrderByDescending(x => x.constructorParameters.Length)
                .FirstOrDefault();

            if (invokaleConstructorInfo == null)
            {
                throw new ActivationException($"Unable to activate type {type}. No matching constructor was found with provided parameters {string.Join(", ", parameters.Select(x => x.GetType().ToString()))}");
            }

            return invokaleConstructorInfo.constructor.Invoke(invokaleConstructorInfo.matchedParameters);
        }

        public T Create<T>(params object[] parameters)
        {
            return (T)Create(typeof(T), parameters);
        }
    }


    public class ActivationException : Exception
    {
        public ActivationException()
        {
        }

        public ActivationException(string message) : base(message)
        {
        }

        public ActivationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}