using Microsoft.Practices.Unity;

namespace WebApiTemplate
{
    public class Factory
    {
        private static IUnityContainer _container;
        public void Configure(IUnityContainer container)
        {
            _container = container;
            RegisterTypes(container);
        }
        public static void RegisterInstance<T>(T item)
        {
            _container.RegisterInstance(item);
        }
        private static void RegisterTypes(IUnityContainer container)
        {
            // ToDo: registreer hier de nodige types op volgende manier (controllers moeten niet geregistreerd worden)
            container.RegisterType<IRequestState, PerHttpRequestState>();
        }
    }
}