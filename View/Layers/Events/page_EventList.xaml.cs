using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using college_events_desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace college_events_desktop.View.Layers.Events
{
	public partial class page_EventList : Page
	{
        #region Поля класса

        private Dictionary<int, bool> filterStates = new Dictionary<int, bool>()
        {
            //я уже не помню чем я руководствовался, когда ключ представлял в виде строки
            //в теории, можно поменять на int, но у этого есть скрытые зависимости в связанных страницах и элементах
            //их тоже важно изменить
            {EventConstants.status_suggested,           false},
            {EventConstants.status_applied,             false},
            {EventConstants.status_done_report_needed,  false},
            {EventConstants.status_done,                false},
            {EventConstants.status_rescheduled,         false},
            {EventConstants.status_rejected,            false},
            {EventConstants.status_suggested_conflict,  false},
        };

        private MainWindow mainWindow;
        EventListViewModel viewModel;
        private DataService _dataService;

        
        #endregion



        #region Конструктор
        public page_EventList(Window win, DataService dataService)
		{
			InitializeComponent();
            ApplySettings();
            Loaded += Page_EventList_Loaded;


            mainWindow = win as MainWindow;
            _dataService = dataService;
            viewModel = new EventListViewModel(mainWindow, this, _dataService);
        }
        #endregion



        #region Обработчики событий
        private async void Page_EventList_Loaded(object sender, RoutedEventArgs e)
        {
            await Update(); //каждый раз, когда визуально отрисовывается окно, обновляем страницу
            mainWindow.Title = StringConstants.mainWindow_Title_EventsList;
        }

        //обновление страницы
        internal async Task Update()
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                using (LoadingService.StartLoading())
                {
                    var caughtErrors = new List<(string UserMessage, Exception Ex)>();
                    try
                    {
                        await _dataService.LoadEventsAsync();
                        viewModel.LoadEventsInStack(_dataService.events);
                    }
                    catch (Exception ex)
                    {
                        caughtErrors.Add(($"произошла ошибка при получении списка мероприятий.", ex));
                    }
                    try
                    {
                        combobox_categories.SelectionChanged -= combobox_categories_SelectionChanged;
                        await _dataService.LoadCategoriesAsync();
                        viewModel.LoadCategoryCombobox(_dataService.categories);
                        combobox_categories.SelectionChanged += combobox_categories_SelectionChanged;

                    }
                    catch (Exception ex)
                    {
                        caughtErrors.Add(($"произошла ошибка при получении списка направлений.", ex));
                    }
                    if (caughtErrors.Count > 0)
                    {
                        UserNotificationService.ShowError("Список ошибок:", caughtErrors, "page_EventListAA002");
                    }
                    UpdateBottomCounters();
                    await apply_filter();
                }
            });
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }


        /// <summary>
        /// Применение фильтра к списку мероприятий. Все кнопки фильтра статусов обращаются именно к нему.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        private async void btn_any_filter_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                //определяем, какой был активирован статус фильтрации
                //в тэге хранится PK статуса (от -1 до 4)
                int colorKey = (int)button.Tag;
                //инвертируем value в словаре фильтров (filterStates)
                filterStates[colorKey] = !filterStates[colorKey];

                //применяем фильтр
                await apply_filter();
                //анимируем цвета кнопки
                await AnimateButtonColorsAsync(button, filterStates[colorKey]);
                SaveSettingValueByKey(colorKey, filterStates[colorKey]);
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Произошла ошибка применения фильтра поиска.", ex, "page_EventListAA001");
            }
        }

        /// <summary>
        /// Применение фильтра к списку мероприятий. На этот метод ссылается только текстовое поле поиска по содержанию.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        private void search_textChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;

            string searchText = textBox.SearchText;
            string lowerSearchText = searchText.ToLower().Trim();

            //наличие активного состояния любого фильтра
            bool isFilterActive = filterStates.Any(s => s.Value == true);

            foreach (control_event_element child in stack_events.Children)
            {
                //если мероприятие было уже скрыто с помощью кнопок фильтрации, то пропустим данный control_event_element
                //и если применён хотя бы один из фильтров с помощью кнопок.
                //изначально все value из словаря filterStates равны false.
                //русским языком, если не добавить условие isFilterActive, то он будет "скипать" все мероприятия
                //при ВЫКЛЮЧЕННЫХ "кнопочных" фильтрах, этот обработчик не будет ничего фильтровать
                if (filterStates[(int)child.Tag] == false && isFilterActive) continue;

                //определяем, есть ли в плашке мероприятия инфа, которую мы ввели в TextBox
                bool containsSearchingText = child.text_name.Text.ToLower().Contains(lowerSearchText) || child.text_organizer_name.Text.ToLower().Contains(lowerSearchText) || $"{child.text_start_time.Text.ToLower()} - {child.text_end_time.Text.ToLower()}".Contains(lowerSearchText) || child.text_date.Text.ToLower().Contains(lowerSearchText) || child.text_direction.Text.ToLower().Contains(lowerSearchText) || child.text_place.Text.ToLower().Contains(lowerSearchText);

                //при условии, что текст из TextBox всё таки присутствует в плашке мероприятия, оставим его видимым в списке, иначе убираем визуальное отображение
                child.Visibility = containsSearchingText ? Visibility.Visible : Visibility.Collapsed;
            }
            UpdateBottomCounters();

        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            string searchText = comboBox.SelectedItem.ToString() ?? string.Empty;
            string lowerSearchText = searchText.ToLower().Trim();

            //наличие активного состояния любого фильтра
            bool isFilterActive = filterStates.Any(s => s.Value == true);

            //если мы отчистили поисковое поле, то выведем все мероприятия в соответствии с включенными "кнопочными" фильтрами
            if (searchText == "Не выбрано")
            {
                foreach (control_event_element child in stack_events.Children)
                {
                    child.Visibility = Visibility.Visible;
                }
                UpdateBottomCounters();
                return;
            }

            foreach (control_event_element child in stack_events.Children)
            {
                //если мероприятие было уже скрыто с помощью кнопок фильтрации, то пропустим данный control_event_element
                //и если применён хотя бы один из фильтров с помощью кнопок.
                //изначально все value из словаря filterStates равны false.
                //русским языком, если не добавить условие isFilterActive, то он будет "скипать" все мероприятия
                //при ВЫКЛЮЧЕННЫХ "кнопочных" фильтрах, этот обработчик не будет ничего фильтровать
                if (filterStates[(int)child.Tag] == false && isFilterActive) continue;

                //определяем, есть ли в плашке мероприятия инфа, которую мы ввели в combobox
                bool containsSearchingText = child.text_direction.Text.ToLower().Contains(lowerSearchText);

                //при условии, что текст из TextBox всё таки присутствует в плашке мероприятия, оставим его видимым в списке, иначе убираем визуальное отображение
                child.Visibility = containsSearchingText ? Visibility.Visible : Visibility.Collapsed;
            }
            UpdateBottomCounters();
        }

        private async void update_page_Click(object sender, RoutedEventArgs e)
        {
            await Update();
        }

        private void go_EventListHelp(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.Navigate(new page_EventListHelp(mainWindow));
        }

        #region border_moving_panel
        private Point _startPoint;
        private double _originX;
        private double _originY;
        private void border_moving_panel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                PanelTransform.X = 0;
                PanelTransform.Y = 0;

                return; // Прерываем выполнение, чтобы не начинать перетаскивание
            }
            _startPoint = e.GetPosition(this);

            // Фиксируем текущий сдвиг панели
            _originX = PanelTransform.X;
            _originY = PanelTransform.Y;

            // Захватываем мышь элементом, чтобы движение не прерывалось
            border_moving_panel.CaptureMouse();

            // Динамически подписываемся на перемещение и отпускание мыши
            border_moving_panel.MouseMove += Border_moving_panel_MouseMove; ;
            border_moving_panel.MouseLeftButtonUp += Border_moving_panel_MouseLeftButtonUp; ;
        }
        private void Border_moving_panel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            border_moving_panel.ReleaseMouseCapture();
            border_moving_panel.MouseMove -= Border_moving_panel_MouseMove;
            border_moving_panel.MouseLeftButtonUp -= Border_moving_panel_MouseLeftButtonUp;
            Properties.Settings.Default.eventList_moving_panel_X = PanelTransform.X;
            Properties.Settings.Default.eventList_moving_panel_Y = PanelTransform.Y;
            Properties.Settings.Default.Save();
        }
        private void Border_moving_panel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (border_moving_panel.IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(this);

                // Вычисляем потенциальные новые координаты
                double deltaX = currentPoint.X - _startPoint.X;
                double deltaY = currentPoint.Y - _startPoint.Y;

                double newX = _originX + deltaX;
                double newY = _originY + deltaY;

                // --- ЗАЩИТА ОТ ВЫЛЕТА ЗА ГРАНИЦЫ ---

                // Получаем размеры страницы и самой панели
                double pageWidth = this.ActualWidth;
                double pageHeight = this.ActualHeight;
                double panelWidth = border_moving_panel.ActualWidth;
                double panelHeight = border_moving_panel.ActualHeight;

                // Так как у вас HorizontalAlignment="Center" и VerticalAlignment="Bottom" с Margin="0,0,0,10",
                // начальное положение панели (когда X=0, Y=0) находится внизу по центру. Нам нужно рассчитать границы относительно этой точки.

                double maxLeft = -(pageWidth - panelWidth) / 2;
                double maxRight = (pageWidth - panelWidth) / 2;

                // Снизу мешает Margin 10 пикселей, сверху — вся высота страницы за вычетом высоты панели и маргина
                double maxDown = 10;
                double maxUp = -(pageHeight - panelHeight - 10);

                // Ограничиваем X и Y в безопасных пределах
                if (newX < maxLeft) newX = maxLeft;
                if (newX > maxRight) newX = maxRight;
                if (newY < maxUp) newY = maxUp;
                if (newY > maxDown) newY = maxDown;

                // ------------------------------------

                // Применяем уже безопасное смещение
                PanelTransform.X = newX;
                PanelTransform.Y = newY;
            }
        }
        #endregion

        #endregion



        #region Методы класса

        /// <summary>
        /// Метод перекрашивания кнопки. Может принадлежать только кнопкам с двумя состояниями: включена, выключена. Метод очень конченый, лучше придумать что-то своё.
        /// </summary>
        /// <param name="button">Объект, требующий анимирования</param>
        /// <param name="statusSwitch">Указатель, в какой цвет перекрашивать кнопку: true - из белого в синий; false - из синего в белый</param>
        /// <returns>Асинхронное выполнение метода</returns>
        private async Task AnimateButtonColorsAsync(Button button, bool statusSwitch)
        {
            if (statusSwitch)
            {
                await UIAnimations.ChangeColorAsync(button, Color.FromArgb(255, 0, 140, 255), 200, EasingMode.EaseOut);
                return;
            }
            await UIAnimations.ChangeColorAsync(button, Colors.White, 200, EasingMode.EaseOut);
            return;
        }

        /// <summary>
        /// Метод применения фильтра статуса мероприятия. Работает в коопе с btn_any_filter_click()
        /// </summary>
        /// <returns>Асинхронное выполнение метода</returns>
        private async Task apply_filter()
        {
            
            bool hasActiveFilters = false;
            foreach (var state in filterStates)
            {
                if (state.Value)
                {
                    hasActiveFilters = true;
                    break;
                }
            }

            //отключаем верстку панели на время массового изменения
            //это предотвращает Layout Pass на каждый шаг цикла!
            stack_events.Visibility = Visibility.Hidden;

            if (hasActiveFilters)
            {
                foreach (UIElement child in stack_events.Children)
                {
                    if (child is control_event_element eventChild && eventChild.Tag != null)
                    {
                        int tagStr = (int)eventChild.Tag;

                        // Проверяем, есть ли такой ключ в словаре, чтобы избежать Crash
                        if (filterStates.TryGetValue(tagStr, out bool isVisible))
                        {
                            Visibility targetVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;

                            // Меняем только если значение РЕАЛЬНО отличается (экономит ресурсы)
                            if (eventChild.Visibility != targetVisibility)
                            {
                                eventChild.Visibility = targetVisibility;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (UIElement child in stack_events.Children)
                {
                    if (child.Visibility != Visibility.Visible)
                    {
                        child.Visibility = Visibility.Visible;
                    }
                }
            }

            //возвращаем панель в строй (WPF пересчитает экран ОДИН раз для всех элементов)
            stack_events.Visibility = Visibility.Visible;
            
            UpdateBottomCounters();
            await Task.Yield();
        }

        /// <summary>
        /// Обновление показателей отображённых мероприятий в списке по статусам
        /// </summary>
        private void UpdateBottomCounters()
        {
            //находим все отобажённые элементы
            var all_control_event_element = stack_events.Children.OfType<control_event_element>().Where(e => e.Visibility == Visibility.Visible).ToList();
            
            //выводим количество всех отображённых элементов
            text_amount_of_events.Text = all_control_event_element.Count().ToString();

            //выводим количество элементов с тэгом = EventConstants.status_done
            text_amount_of_gray.Text = all_control_event_element.Count(g => 
                (int)g.Tag == EventConstants.status_done).ToString();

            //выводим количество элементов с тэгом = EventConstants.status_done_report_needed
            text_amount_of_blue.Text = all_control_event_element.Count(bl => 
                (int)bl.Tag == EventConstants.status_done_report_needed).ToString();

            //выводим количество элементов с тэгом = EventConstants.status_applied или EventConstants.status_rescheduled
            text_amount_of_green.Text = all_control_event_element.Count(gr => 
                (int)gr.Tag == EventConstants.status_applied || (int)gr.Tag == EventConstants.status_rescheduled).ToString();

            //выводим количество элементов с тэгом = EventConstants.status_suggested
            text_amount_of_yellow.Text = all_control_event_element.Count(y => 
                (int)y.Tag == EventConstants.status_suggested).ToString();

            //выводим количество элементов с тэгом = EventConstants.status_suggested_conflict
            text_amount_of_red.Text = all_control_event_element.Count(r => 
                (int)r.Tag == EventConstants.status_suggested_conflict).ToString();
        }


        /// <summary>
        /// Возвращает состояние фильтра по статусу из памяти
        /// </summary>
        /// <param name="key">Идентификатор статуса</param>
        private bool GetSettingValueByKey(int key)
        {
            switch (key)
            {
                case EventConstants.status_suggested_conflict: 
                    return Properties.Settings.Default.status_filter_red;

                case EventConstants.status_suggested: 
                    return Properties.Settings.Default.status_filter_yellow;

                case EventConstants.status_applied: 
                    return Properties.Settings.Default.status_filter_green;

                case EventConstants.status_done_report_needed: 
                    return Properties.Settings.Default.status_filter_blue;

                case EventConstants.status_done: 
                    return Properties.Settings.Default.status_filter_grey;

                default: return false;
            }
        }


        /// <summary>
        /// Сохраняет состояние фильтра по статусу в память
        /// </summary>
        /// <param name="key">Идентификатор статуса</param>
        /// <param name="value">Сохраняемое значение</param>
        private void SaveSettingValueByKey(int key, bool value)
        {
            switch (key)
            {
                case EventConstants.status_suggested_conflict: 
                    Properties.Settings.Default.status_filter_red = value; 
                    break;

                case EventConstants.status_suggested: 
                    Properties.Settings.Default.status_filter_yellow = value; 
                    break;

                case EventConstants.status_applied: 
                    Properties.Settings.Default.status_filter_green = value; 
                    break;

                case EventConstants.status_done_report_needed: 
                    Properties.Settings.Default.status_filter_blue = value; 
                    break;

                case EventConstants.status_done: 
                    Properties.Settings.Default.status_filter_grey = value; 
                    break;
            }
            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// Метод применяет пользовательские настройки (при старте приложения)
        /// </summary>
        private async void ApplySettings()
        {
            //применение трансформации (последнего местоположения) к плавающей панели
            PanelTransform.X = Properties.Settings.Default.eventList_moving_panel_X;
            PanelTransform.Y = Properties.Settings.Default.eventList_moving_panel_Y;
            
            //включение последних включенных фильтров
            filterStates[EventConstants.status_suggested_conflict] = GetSettingValueByKey(EventConstants.status_suggested_conflict);
            filterStates[EventConstants.status_suggested] = GetSettingValueByKey(EventConstants.status_suggested);
            filterStates[EventConstants.status_applied] = GetSettingValueByKey(EventConstants.status_applied);
            filterStates[EventConstants.status_done_report_needed] = GetSettingValueByKey(EventConstants.status_done_report_needed);
            filterStates[EventConstants.status_done] = GetSettingValueByKey(EventConstants.status_done);
            //визуально включаем фильтры (перекрашиваем те кнопки, что включены)
            await AnimateButtonColorsAsync(btn_red_filter, filterStates[EventConstants.status_suggested_conflict]);
            await AnimateButtonColorsAsync(btn_yellow_filter, filterStates[EventConstants.status_suggested]);
            await AnimateButtonColorsAsync(btn_green_filter, filterStates[EventConstants.status_applied]);
            await AnimateButtonColorsAsync(btn_blue_filter, filterStates[EventConstants.status_done_report_needed]);
            await AnimateButtonColorsAsync(btn_gray_filter, filterStates[EventConstants.status_done]);
        }
        #endregion
    }
}