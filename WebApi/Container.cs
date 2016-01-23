using System;
using Microsoft.Practices.Unity;

namespace WebApiTemplate
{
    public class Container
    {
        public static IUnityContainer UnderlyingContainer;

        public static void InitializeWith(IUnityContainer container)
        {
            UnderlyingContainer = container;
        }

        public static T Resolve<T>()
        {
            return UnderlyingContainer.Resolve<T>();
        }
        public static T Resolve<T>(string key)
        {
            return UnderlyingContainer.Resolve<T>(key);
        }

        public static object Resolve(Type type)
        {
            return UnderlyingContainer.Resolve(type);
        }
    }
}