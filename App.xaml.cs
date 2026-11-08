using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace college_events_desktop
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            Services = serviceCollection.BuildServiceProvider();
            ServiceLocator.Initialize(Services);

            // Запуск главного окна входа
            var loginWindow = Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 1. HttpClient без автоматических хендлеров (пока)
            services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7280/college/admin/");
            });

            // 2. Сервисы
            services.AddSingleton<DataService>();
            services.AddSingleton<OverlayService>();

            // 3. Окна
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();

            // 4. ViewModels (если начнете переводить)
            // services.AddTransient<EventListViewModel>();
        }



        // регионы оставил тут для вставки в новые файлы, потому что я за****ся писать region, endregion
        #region Поля класса
        #endregion

        #region Конструктор
        #endregion

        #region Обработчики событий
        #endregion

        #region Методы класса
        #endregion
    }
}
