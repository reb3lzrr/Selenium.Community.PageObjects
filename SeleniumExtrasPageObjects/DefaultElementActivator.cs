using System;
using System.Linq;
using System.Reflection;

namespace SeleniumExtras.PageObjects
{
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
                                //var constructorParameterType = y.ParameterType

                                return parameters
                                    .FirstOrDefault(z => z.GetType() == y.ParameterType || z.GetType().GetInterfaces().Intersect(y.ParameterType.GetInterfaces()).Any());

                        
                            })
                            .Where(y => y != null)
                            .ToArray()
                    };
                })
                .Where(x => x.matchedParameters.Length == x.constructorParameters.Length)
                .OrderByDescending(x => x.matchedParameters.Count());


                

            return ctorInfo.First().constructor.Invoke(ctorInfo.First().matchedParameters);
        }


        public T Create<T>(params object[] parameters)
        {
            return (T)Create(typeof(T), parameters);
        }

    }
}