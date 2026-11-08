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

            var loginWindow = Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //регистрируем Http-клиент, но пока без перехватчика, т.к. он зависит от токен-сервиса
            services.AddTransient<AuthHeaderHandler>();

            services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7280/college/admin/");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>(); // Подключаем перехватчик!

            // Отдельный "голый" HttpClient для AuthTokenService, чтобы избежать рекурсии 
            // (мы же не хотим, чтобы запрос на refresh токен тоже пытался обновить токен)
            services.AddHttpClient<AuthTokenService>("auth", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7280/college/admin/");
            });

            services.AddSingleton<SessionService>();
            services.AddSingleton<DataService>();

            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
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
