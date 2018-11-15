using System;

namespace SeleniumExtras.PageObjects
{
    public interface IElementActivator
    {
        object Create(Type type, params object[] parameters);

        T Create<T>(params object[] parameters);
    }
}
