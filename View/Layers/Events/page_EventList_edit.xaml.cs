using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Layers.Events.Tables;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace college_events_desktop.View.Layers.Events
{
    public partial class page_EventList_edit : Page
    {
        #region Поля класса

        internal EventUpdateDto EventUpdateData 
        { 
            get => new EventUpdateDto
            {
                Event = new Event
                {
                    eventId = _Event.eventId,
                    title = edit_event_name.Text,
                    startDateTime = datePicker_date.SelectedDate.GetValueOrDefault().Add(Convert.ToDateTime(edit_startTime.Text).TimeOfDay),
                    endDateTime = datePicker_date.SelectedDate.GetValueOrDefault().Add(Convert.ToDateTime(edit_endTime.Text).TimeOfDay),
                    fullDescription = edit_fullDescription.Text,
                    shortDescription = edit_shortDescription.Text,
                    organizerId = GetOrganizerId(combobox_organizer_name.Text),
                    categoryId = GetCategoryId(combobox_event_direction.Text),
                    eventLocationsIds = locations.Select(l => l.locationId).ToList(),
                    additionalInfo = edit_additionalInfo.Text,
                    maxListenersCount = Convert.ToInt32(edit_maxListenersCount.Text),
                    maxParticipantsCount = Convert.ToInt32(edit_maxParticipantsCount.Text),
                    organizerOrganization = edit_organizerOrganization.Text,
                    organizerPosition = edit_organizerPosition.Text,
                },
                Groups = _table.EventGroups
            }; 
        }
        private List<Location> locations => stack_places.Children
            .OfType<Button>()
            .Select(button => new Location
            {
                locationId = (int)button.Tag,
                place = button.Content?.ToString() ?? string.Empty
            })
            .ToList();


        private MainWindow mainWindow;
        private DataService _dataService;
        private Event _Event;
        page_table_EventGroup_edit _table;

        private readonly IOverlayService _overlayService; //оверлей, в который всё чо угодно пихаешь, будет поверх основного окна
        #endregion





        #region Конструктор класса
        public page_EventList_edit(Window win, DataService dataService, Event _event)
        {
            InitializeComponent();
            mainWindow = win as MainWindow;
            _dataService = dataService;
            _Event = _event;
            DataContext = _Event;
            _table = new page_table_EventGroup_edit(this, _dataService, _Event);

            _overlayService = new OverlayService(mainWindow);

            Loaded += Page_EventList_edit_Loaded;
        }
        #endregion





        #region Обработчики событий
        private async void Page_EventList_edit_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadInformation(); //загрузка всей инфы, что мне было лень добавлять через сложные привязки + обработка исключений
            frame_table.Navigate(_table);
            event_status.Background = _Event.statusColor;
            
        }


        private void combobox_event_place_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem == null) { return; }

            string text = comboBox.SelectedItem.ToString();

            foreach (Button place in stack_places.Children.OfType<Button>())
            {
                if ((string)place.Content == text)
                {
                    MessageBox.Show("Это место уже добавлено.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    comboBox.SelectedItem = null;
                    
                    return;
                }
            }
            stack_places.Children.Insert(stack_places.Children.Count - 1, CreateLocationButton(_dataService.places.FirstOrDefault(p => p.place == text)));
            comboBox.SelectedItem = null;
        }


        private void goback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.GoBack();
        }


        public int validation_errors_count = 0; //используется (как минимум) в 'table_tuple_EventGroup_edit.cs' в методе 'edit_expectedCount_Error()' для счёта кол-ва ошибок в введённых данных пользователем
        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            btn_save.IsEnabled = false;
            using (LoadingService.StartLoading())
            {
                try
                {
                    if (validation_errors_count > 0)
                    {
                        UserNotificationService.ShowWarning($"У вас есть ошибки ввода. Пожалуйста, исправьте их ({validation_errors_count} ошибок) перед выполнением сохранения.\n\nПодсказка:\n1. Количество участников не может быть отрицательным или содержать символы кроме цифр");
                    }
                    else
                    {
                        bool response = await _dataService.apiClient.UpdateEventGroups(EventUpdateData, _Event.eventId);
                        if (!response)
                        {
                            UserNotificationService.ShowError("Произошла ошибка обновления.", "page_EventList_editAA003");
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    UserNotificationService.ShowError("Возникла ошибка отправки запроса на сервер.", ex, "page_EventList_editAA001");
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Возникла непредвиденная ошибка.", ex, "page_EventList_editAA002");
                }
            }
            btn_save.IsEnabled = true;
        }

        private void btn_add_new_place_Click(object sender, RoutedEventArgs e)
        {
            _overlayService.Open(new page_NewLocation(mainWindow));
        }

        #endregion





        #region Методы класса
        private async Task LoadInformation()
        {
            await Application.Current.Dispatcher.InvokeAsync(async () => 
            { 
                //данные о мероприятии
                combobox_organizer_name.Text = $"{_Event.organizerSurname} {_Event.organizerName} {_Event.organizerLastname}";
                combobox_event_direction.Text = _Event.categoryName;

                // добавление мест проведения мероприятия (именно что кнопок - конкретно зафиксированных мест для мероприятия)
                _Event.locations.ForEach(location => stack_places.Children.Insert(stack_places.Children.Count - 1, CreateLocationButton(location)));

                //подгрузка информации из БД
                await _dataService.LoadGroupsListAsync();
                await _dataService.LoadPlacesListAsync();
                await _dataService.LoadOrganizerListAsync();

                var caughtErrors = new List<(string UserMessage, Exception Ex)>();
                try
                {
                    ///добавление общего списка мест проведения мероприятий (все доступные в системе места для проведения - комбобокс для добавления)
                    string[] places = _dataService.places.Select(p => p.place).ToArray();
                    combobox_event_place.ItemsSource = places;
                }
                catch (Exception ex)
                {
                    caughtErrors.Add(($"произошла ошибка при получении полного списка локаций.", ex));
                }

                try
                {
                    ///аналогично. добавление фулл списка огранизаторов в комбобокс
                    string[] organizers = _dataService.organizers
                        .Select(o => $"{o.surName} {o.firstName} {o.lastName}".Trim())
                        .ToArray();
                    combobox_organizer_name.ItemsSource = organizers;
                }
                catch (Exception ex)
                {
                    caughtErrors.Add(($"произошла ошибка при получении полного списка организаторов.", ex));
                }

                try
                {
                    ///аналогично. добавление фулл списка категорий в комбобокс
                    string[] categories = _dataService.categories.Select(c => c.name).ToArray();
                    combobox_event_direction.ItemsSource = categories;
                }
                catch (Exception ex)
                {
                    caughtErrors.Add(($"произошла ошибка при получении полного списка категорий.", ex));
                }

                //показ ошибки, если что-то пошло не так
                if (caughtErrors.Count > 0)
                {
                    UserNotificationService.ShowError("Список ошибок:", caughtErrors, "page_EventList_editAA004");
                }
            
            });
        }

        //TODO: CODE REVEAL: поместить в отдельный класс
        /// <summary>
        /// Метод, позволяющий из строки ФИО (ПОРЯДОК ПОЛНОГО ИМЕНИ ОБЯЗАТЕЛЕН ЧЕРЕЗ ПРОБЕЛ) определить ID организатора в БД
        /// </summary>
        /// <param name="content">строка ФИО</param>
        /// <returns>уникальный идентификатор организатора</returns>
        private int GetOrganizerId(string content)
        {
            string organizerName = content.Split(' ')[1];
            string organizerSurName = content.Split(' ')[0];
            string organizerLastName = content.Split(' ')[2];
            try
            {
                var response = _dataService.organizers.Find(o => o.firstName == organizerName && o.surName == organizerSurName && o.lastName == organizerLastName).userId;
                return response;
            }
            catch
            {
                throw new Exception("У пользователя нет роли организатора. Пожалуйста, добавьте роль пользователю для продолжения.");
            }
        }

        //TODO: CODE REVEAL: поместить в отдельный класс
        /// <summary>
        /// Метод, позволяющий из строки с наименованием категории (направления) определить ID в БД
        /// </summary>
        /// <param name="content">категория (направление)</param>
        /// <returns>уникальный идентификатор категории</returns>
        private int GetCategoryId(string content)
        {
            return _dataService.categories.Find(c => c.name == content).categoryId;
        }


        //TODO: CODE REVEAL: поместить в отдельный класс
        /// <summary>
        /// метод для создания кнопки с локацией. НЕ УНИВЕРСАЛЬНЫЙ! НЕ ПОДХОДИТ ДЛЯ ЛЮБОГО КОНТЕЙНЕРА!
        /// </summary>
        /// <param name="location">Класс локации</param>
        /// <returns>кнопка</returns>
        private Button CreateLocationButton(Location location)
        {
            var btn = new Button()
            {
                Tag = location.locationId,
                Content = location.place,
                Style = (Style)TryFindResource("ButtonStyle_X"),
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Padding = new Thickness(5, 1, 5, 1),
                Margin = new Thickness(5),
                MinWidth = 80,
                FontSize = 14,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            //не универсальный, потому что событие нажатия строго определено для удаления из stack_places
            btn.Click += (s, e) => stack_places.Children.Remove(btn);
            return btn;
        }
        #endregion

    }
}
