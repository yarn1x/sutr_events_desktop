using college_events_desktop.View.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly SessionService _session;
        private readonly AuthTokenService _tokenService;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // Чтобы 10 запросов разом не обновляли токен 10 раз

        public AuthHeaderHandler(SessionService session, AuthTokenService tokenService)
        {
            _session = session;
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //прикрепляем текущий Access-токен к запросу
            if (!string.IsNullOrEmpty(_session.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.AccessToken);
            }

            //отправляем запрос
            var response = await base.SendAsync(request, cancellationToken);

            //если сервер сказал 401
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //блокируем остальные потоки, чтобы они не начали обновлять токен одновременно
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    //проверяем, не обновил ли токен другой поток, пока мы ждали
                    if (request.Headers.Authorization?.Parameter != _session.AccessToken)
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.AccessToken);
                        return await base.SendAsync(request, cancellationToken); // Повторяем запрос с новым токеном
                    }

                    //пытаемся обновить
                    if (await _tokenService.RefreshTokenAsync())
                    {
                        //если обновили — повторяем исходный запрос с новым токеном
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.AccessToken);
                        response.Dispose(); //старый ответ 401 нам больше не нужен
                        return await base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        //если обновить не удалось (refresh протух) — сессия мертва
                        _session.Clear();
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            //закрываем главное окно и открываем логин
                            var login = App.Services.GetRequiredService<LoginWindow>();
                            login.Show();
                            App.Current.MainWindow?.Close();
                        });
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return response;
        }
    }
}
