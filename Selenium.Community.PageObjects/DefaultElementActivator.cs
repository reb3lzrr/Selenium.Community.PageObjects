using System;
using System.Linq;

namespace Selenium.Community.PageObjects
{
    /// <summary>
    /// A default activator used by the <see cref="PageObjectFactory"/>
    /// </summary>
    public class DefaultElementActivator : IElementActivator
    {
        private readonly object[] _additionalParameters;

        /// <summary>
        /// Creates a new instance of DefaultElementActivator
        /// </summary>
        /// <param name="additionalParameters"></param>
        public DefaultElementActivator(params object[] additionalParameters)
        {
            _additionalParameters = additionalParameters;
        }

        /// <inheritdoc />
        public object Create(Type type, params object[] parameters)
        {
            parameters = parameters.Concat(_additionalParameters).ToArray();

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
                        .ToArray(),
                    unmatchedParameters = x.GetParameters().Where(y => !availableTypesDictionary.ContainsKey(y.ParameterType))
                        .Select(y => y)
                        .ToArray(),
                })
                .ToArray();

            var invokableConstructorInfo = constructors
                .OrderByDescending(x => x.constructorParameters.Length)
                .FirstOrDefault(x => x.matchedParameters.Count() == x.constructorParameters.Length);

            if (invokableConstructorInfo == null)
            {
                var bestMatchConstructor = constructors
                    .OrderBy(x => x.constructorParameters.Length)
                    .First();
                var unresolvedParameter = bestMatchConstructor.unmatchedParameters.First();

                throw new ActivationException($"Cannot resolve parameter '{unresolvedParameter.ParameterType} {unresolvedParameter.Name}' of constructor 'Void .ctor({string.Join(", ", bestMatchConstructor.constructorParameters.Select(x => x.ParameterType))})'.");
            }

            return invokableConstructorInfo.constructor.Invoke(invokableConstructorInfo.matchedParameters);
        }

        /// <inheritdoc />
        public T Create<T>(params object[] parameters)
        {
            return (T)Create(typeof(T), parameters);
        }
    }
}