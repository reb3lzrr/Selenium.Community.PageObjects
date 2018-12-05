using System;
using System.Linq;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// A default activator for use with the <see cref="PageObjectFactory"/>. This implementation 
    /// </summary>
    public class DefaultElementActivator : IElementActivator
    {
        public object Create(Type type, params object[] parameters)
        {
            var ctors = type.GetConstructors();
            var ctorInfo = ctors
                .Select(x =>
                {
                    var ctorParams = x.GetParameters();

                    return new
                    {
                        constructor = x,
                        constructorParameters = ctorParams,
                        matchedParameters = ctorParams
                            .Select(y =>
                            {
                                return parameters
                                    .FirstOrDefault(z => z.GetType() == y.ParameterType || z.GetType().GetInterfaces().Intersect(y.ParameterType.GetInterfaces()).Any());
                            })
                            .Where(y => y != null)
                            .ToArray()
                    };
                })
                .Where(x => x.matchedParameters.Length == x.constructorParameters.Length)
                .OrderByDescending(x => x.matchedParameters.Length)
                .FirstOrDefault();

            if (ctorInfo == null)
            {
                throw new Exception($"Unable to find a matching constructor on type {type} with provided parameters {string.Join(", ", parameters.Select(x => x.GetType().ToString()))}");
            }

            return ctorInfo.constructor.Invoke(ctorInfo.matchedParameters);
        }

        public T Create<T>(params object[] parameters)
        {
            return (T)Create(typeof(T), parameters);
        }

    }
}