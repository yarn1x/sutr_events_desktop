using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop
{
    public static class ServiceLocator
    {
        private static IServiceProvider _provider;
        public static IServiceProvider Provider => _provider;

        public static void Initialize(IServiceProvider provider)
        {
            _provider = provider;
        }

        // Удобный метод для получения сервиса: ServiceLocator.Get<DataService>()
        public static T Get<T>() => _provider.GetRequiredService<T>();
    }
}
